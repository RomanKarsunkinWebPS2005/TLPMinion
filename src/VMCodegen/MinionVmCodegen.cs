using System.Globalization;

using TLPMinion.Ast;
using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Ast.Statements;
using TLPMinion.Runtime;
using TLPMinion.VirtualMachine.Builtins;
using TLPMinion.VirtualMachine.Instructions;

namespace TLPMinion.VMCodegen;

using VType = TLPMinion.Runtime.ValueType;

public sealed class MinionVmCodegen : IAstVisitor
{
    private readonly InstructionsBuilder _builder = new();
    private CodegenSymbolsTable? _symbols;

    public IReadOnlyList<Instruction> GenerateCode(Expression program)
    {
        program.Accept(this);
        if (program.ResultType != VType.Void)
        {
            _builder.Append(new Instruction(InstructionCode.StoreResult));
        }

        _builder.Append(new Instruction(InstructionCode.Push, 0));
        _builder.Append(new Instruction(InstructionCode.Halt));
        return _builder.Finish();
    }

    public void Visit(ConstDeclaration declaration)
    {
        Value value = declaration.CompileTimeValue
            ?? throw new InvalidOperationException("Для const не вычислено compile-time значение.");
        if (value.IsInt())
        {
            _builder.Append(new Instruction(InstructionCode.Push, value.AsInt()));
        }
        else
        {
            _builder.Append(new Instruction(InstructionCode.Push, value));
        }

        _builder.Append(new Instruction(InstructionCode.DefineVar, declaration.Name));
    }

    public void Visit(VariableDeclaration declaration)
    {
        declaration.Initializer.Accept(this);
        _builder.Append(new Instruction(InstructionCode.DefineVar, declaration.Name));
    }

    public void Visit(AssignmentExpression expression)
    {
        if (expression.Left is not IdentifierExpression identifier)
        {
            throw new NotSupportedException("Ожидается присваивание в переменную.");
        }

        expression.Right.Accept(this);
        _builder.Append(new Instruction(InstructionCode.StoreVar, identifier.Name));
        _builder.Append(new Instruction(InstructionCode.LoadVar, identifier.Name));
    }

    public void Visit(BinaryExpression expression)
    {
        switch (expression.Operator)
        {
            case BinaryOperator.Greater:
                expression.Right.Accept(this);
                expression.Left.Accept(this);
                _builder.Append(new Instruction(InstructionCode.Less));
                return;
            case BinaryOperator.GreaterOrEqual:
                expression.Right.Accept(this);
                expression.Left.Accept(this);
                _builder.Append(new Instruction(InstructionCode.LessOrEqual));
                return;
            case BinaryOperator.And:
                EmitShortCircuitAnd(expression);
                return;
            case BinaryOperator.Or:
                EmitShortCircuitOr(expression);
                return;
            default:
                expression.Left.Accept(this);
                expression.Right.Accept(this);
                _builder.Append(new Instruction(MapBinary(expression.Operator)));
                break;
        }
    }

    public void Visit(ConditionalExpression expression)
    {
        expression.Condition.Accept(this);
        BasicBlock falseBlock = _builder.CreateBasicBlock();
        BasicBlock endBlock = _builder.CreateBasicBlock();
        _builder.AppendJump(InstructionCode.JumpIfFalse, falseBlock);
        expression.WhenTrue.Accept(this);
        _builder.AppendJump(InstructionCode.Jump, endBlock);
        _builder.InsertPoint = falseBlock;
        expression.WhenFalse.Accept(this);
        _builder.InsertPoint = endBlock;
    }

    public void Visit(FunctionCallExpression expression)
    {
        if (expression.Function is not BuiltinFunction)
        {
            throw new NotSupportedException($"Вызов '{expression.Name}' не поддержан");
        }

        foreach (Expression argument in expression.Arguments)
        {
            argument.Accept(this);
        }

        BuiltinFunctionCode code = MapBuiltinFunction(expression.Name);
        _builder.Append(new Instruction(InstructionCode.CallBuiltin, (int)code));
    }

    public void Visit(InputStatement statement)
    {
        IdentifierExpression id = statement.Target;
        BuiltinFunctionCode code = MapInput(id);
        _builder.Append(new Instruction(InstructionCode.CallBuiltin, (int)code));
        _builder.Append(new Instruction(InstructionCode.StoreVar, id.Name));
    }

    public void Visit(PrintStatement statement)
    {
        statement.Argument.Accept(this);
        _builder.Append(new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print));
    }

    public void Visit(IdentifierExpression expression)
    {
        _builder.Append(new Instruction(InstructionCode.LoadVar, expression.Name));
    }

    public void Visit(LiteralExpression expression)
    {
        if (expression.TypeName == Builtins.Int)
        {
            int value = int.Parse(expression.Lexeme, CultureInfo.InvariantCulture);
            _builder.Append(new Instruction(InstructionCode.Push, value));
            return;
        }

        if (expression.TypeName == Builtins.Float)
        {
            double value = double.Parse(expression.Lexeme, CultureInfo.InvariantCulture);
            _builder.Append(new Instruction(InstructionCode.Push, new Value(value)));
            return;
        }

        if (expression.TypeName == Builtins.String)
        {
            _builder.Append(new Instruction(InstructionCode.Push, new Value(expression.Lexeme)));
            return;
        }

        if (expression.TypeName == Builtins.Bool)
        {
            bool value = expression.Lexeme == "true";
            _builder.Append(new Instruction(InstructionCode.Push, new Value(value)));
            return;
        }

        throw new NotSupportedException($"Литерал типа '{expression.TypeName}'.");
    }

    public void Visit(ScopeExpression expression)
    {
        PushScope();
        IReadOnlyList<ScopeItem> members = expression.Members;
        for (int i = 0; i < members.Count; i++)
        {
            ScopeItem item = members[i];
            switch (item)
            {
                case DeclarationScopeItem d:
                    d.Declaration.Accept(this);
                    break;
                case StatementScopeItem s:
                    s.Statement.Accept(this);
                    bool isLast = i == members.Count - 1;
                    if (!isLast && s.Statement.ResultType != VType.Void)
                    {
                        _builder.Append(new Instruction(InstructionCode.Pop));
                    }

                    break;
                default:
                    throw new NotSupportedException($"Неизвестный элемент области: {item.GetType().Name}.");
            }
        }

        PopScope();
    }

    public void Visit(UnaryExpression expression)
    {
        expression.Operand.Accept(this);
        switch (expression.Operator)
        {
            case UnaryOperator.Plus:
                break;
            case UnaryOperator.Minus:
                _builder.Append(new Instruction(InstructionCode.Negate));
                break;
            case UnaryOperator.Not:
                _builder.Append(new Instruction(InstructionCode.Not));
                break;
            default:
                throw new NotSupportedException($"Унарный оператор {expression.Operator}.");
        }
    }

    private void EmitShortCircuitAnd(BinaryExpression expression)
    {
        expression.Left.Accept(this);
        BasicBlock falseBlock = _builder.CreateBasicBlock();
        BasicBlock endBlock = _builder.CreateBasicBlock();
        _builder.AppendJump(InstructionCode.JumpIfFalse, falseBlock);
        expression.Right.Accept(this);
        _builder.AppendJump(InstructionCode.Jump, endBlock);
        _builder.InsertPoint = falseBlock;
        _builder.Append(new Instruction(InstructionCode.Push, new Value(false)));
        _builder.InsertPoint = endBlock;
    }

    private void EmitShortCircuitOr(BinaryExpression expression)
    {
        expression.Left.Accept(this);
        BasicBlock trueBlock = _builder.CreateBasicBlock();
        BasicBlock endBlock = _builder.CreateBasicBlock();
        _builder.AppendJump(InstructionCode.JumpIfTrue, trueBlock);
        expression.Right.Accept(this);
        _builder.AppendJump(InstructionCode.Jump, endBlock);
        _builder.InsertPoint = trueBlock;
        _builder.Append(new Instruction(InstructionCode.Push, new Value(true)));
        _builder.InsertPoint = endBlock;
    }

    private void PushScope()
    {
        int parentDepth = _symbols?.Depth ?? 0;
        _symbols = new CodegenSymbolsTable(_symbols);
        _builder.Append(new Instruction(InstructionCode.PushVars, parentDepth));
    }

    private void PopScope()
    {
        _builder.Append(new Instruction(InstructionCode.PopVars));
        _symbols = _symbols!.Parent;
    }

    private static InstructionCode MapBinary(BinaryOperator op)
    {
        return op switch
        {
            BinaryOperator.Add => InstructionCode.Add,
            BinaryOperator.Subtract => InstructionCode.Subtract,
            BinaryOperator.Multiply => InstructionCode.Multiply,
            BinaryOperator.Divide => InstructionCode.Divide,
            BinaryOperator.Modulo => InstructionCode.Modulo,
            BinaryOperator.Power => InstructionCode.Power,
            BinaryOperator.Equal => InstructionCode.Equal,
            BinaryOperator.NotEqual => InstructionCode.NotEqual,
            BinaryOperator.Less => InstructionCode.Less,
            BinaryOperator.LessOrEqual => InstructionCode.LessOrEqual,
            BinaryOperator.And or BinaryOperator.Or =>
                throw new NotSupportedException($"Оператор {op} требует кодогенерации с переходами."),
            _ => throw new NotSupportedException($"Оператор {op}."),
        };
    }

    private static BuiltinFunctionCode MapInput(IdentifierExpression id)
    {
        return id.Variable.TypeName switch
        {
            Builtins.Int => BuiltinFunctionCode.InputInt,
            Builtins.Float => BuiltinFunctionCode.InputFloat,
            Builtins.String => BuiltinFunctionCode.InputString,
            _ => throw new NotSupportedException($"input для типа '{id.Variable.TypeName}' не поддержан."),
        };
    }

    private static BuiltinFunctionCode MapBuiltinFunction(string name)
    {
        return name switch
        {
            Builtins.Length => BuiltinFunctionCode.StringLength,
            Builtins.Substring => BuiltinFunctionCode.StringSubstring,
            _ => throw new NotSupportedException($"Встроенная функция '{name}' не поддержана."),
        };
    }
}
