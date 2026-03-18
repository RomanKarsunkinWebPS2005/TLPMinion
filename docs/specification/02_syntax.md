# 02. Синтаксис языка

## 1. Операторы

| Приоритет | Операторы | Описание | Ассоциативность |
|:---------:|:---------:|:---------|:---------------:|
| 1 | `()` | Скобки группировки, вызов функции | Слева направо |
| 2 | `!`, `+`, `-` (унарные) | Логическое НЕ, унарный плюс/минус | Справа налево |
| 3 | `**` | Возведение в степень | Справа налево |
| 4 | `*`, `/`, `%` | Умножение, деление, остаток | Слева направо |
| 5 | `+`, `-` | Сложение, вычитание | Слева направо |
| 6 | `<`, `<=`, `>`, `>=` | Операции сравнения | Слева направо |
| 7 | `==`, `!=` | Операции равенства | Слева направо |
| 8 | `&&` | Логическое И | Слева направо |
| 9 | `\|\|` | Логическое ИЛИ | Слева направо |
|10 | `?:` | Тернарный условный оператор | Справа налево |
| 11 | `=` | Присваивание | Справа налево |

## 2. EBNF-грамматика

```ebnf
(* 1. Программа *)
program = { top-level-item } ;
top-level-item = declaration | statement ;

(* 2. Объявления *)
declaration = const-declaration
            | function-declaration
            | variable-declaration ;

const-declaration = "const" , identifier , ":" , type , "=" , expression , ";" ;

function-declaration = "func" , identifier , "(" , [ parameter-list ] , ")" , [ ":" , type ] , block ;

parameter-list = parameter , { "," , parameter } ;

parameter = identifier , ":" , type ;

variable-declaration = ( "var" | "let" ) , identifier , ":" , type , [ "=" , expression ] , ";" ;

type = "Int" | "Float" | "Bool" | "String" | "Void" ;

(* 3. Инструкции *)
statement = variable-declaration
          | assignment
          | expression-statement
          | if-statement
          | while-statement
          | for-statement
          | return-statement
          | print-statement
          | input-statement
          | block ;

block = "{" , { statement } , "}" ;

assignment = identifier , "=" , expression , ";" ;

expression-statement = expression , ";" ;

if-statement = "if" , "(" , expression , ")" , block , [ "else" , ( block | if-statement ) ] ;

while-statement = "while" , "(" , expression , ")" , block ;

for-statement = "for" , "(" , [ initialization ] , ";" , [ expression ] , ";" , [ expression ] , ")" , block ;

initialization = statement | expression ;

return-statement = "return" , [ expression ] , ";" ;

print-statement = "print" , "(" , expression , ")" , ";" ;

input-statement = "input" , "(" , identifier , ")" , ";" ;

(* 4. Выражения *)
expression = assignment-expression ;

assignment-expression = ternary-expression , [ "=" , assignment-expression ] ;

ternary-expression = logical-or-expression , [ "?" , expression , ":" , expression ] ;

logical-or-expression = logical-and-expression , { "||" , logical-and-expression } ;

logical-and-expression = equality-expression , { "&&" , equality-expression } ;

equality-expression = relational-expression , { ( "==" | "!=" ) , relational-expression } ;

relational-expression = additive-expression , { ( "<" | "<=" | ">" | ">=" ) , additive-expression } ;

additive-expression = multiplicative-expression , { ( "+" | "-" ) , multiplicative-expression } ;

multiplicative-expression = power-expression , { ( "*" | "/" | "%" ) , power-expression } ;

power-expression = unary-expression , [ "**" , power-expression ] ;

unary-expression = ( "!" | "+" | "-" ) , unary-expression | primary-expression ;

primary-expression = literal
                   | identifier
                   | function-call
                   | builtin-fanction-call
                   | "(" , expression , ")" ;
function-call = identifier , "(" , [ argument-list ] , ")" ;
argument-list = expression , { "," , expression } ;

builtin-fanction-call = "abs" , "(" , expression , ")"
                        | "min" , "(" , expression , "," , expression , ")"
                        | "max" , "(" , expression , "," , expression , ")" ;

(* 5. Литералы *)
literal = number-literal | string-literal | boolean-literal ;

number-literal = [ sign ] , integer-part , [ fractional-part ];

sign = "-" | "+" ;

integer-part = digit , { digit } ;

fractional-part = "." , digit , { digit } ;

string-literal = '"' , { character | escape-sequence } , '"' ;

character = ? любой печатный символ, кроме '"' и '\' ? ;

escape-sequence = "\" , ( "n" | "\"" | "\\" ) ;

boolean-literal = "true" | "false" ;

(* 6. Лексемы *)
identifier = letter , { letter | digit | "_" } ;
letter = "a" | "b" | "c" | "d" | "e" | "f" | "g" | "h" | "i" | "j" | "k" | "l" | "m" |
         "n" | "o" | "p" | "q" | "r" | "s" | "t" | "u" | "v" | "w" | "x" | "y" | "z" |
         "A" | "B" | "C" | "D" | "E" | "F" | "G" | "H" | "I" | "J" | "K" | "L" | "M" |
         "N" | "O" | "P" | "Q" | "R" | "S" | "T" | "U" | "V" | "W" | "X" | "Y" | "Z" ;
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
```