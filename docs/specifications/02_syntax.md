# 02. Синтаксис языка

## 1. Структура программы

Программа состоит из элементов верхнего уровня, которыми могут быть:
- Объявления констант, переменных или функций.
- Инструкции (вызовы функций, присваивания, управляющие конструкции).

Вся программа глобально находится в одной области видимости, внутри которой могут создаваться вложенные блоки кода.

## 2. Объявления

### 2.1. Объявление констант
 Инициализация константы (`const`) обязательна и должна происходить в момент объявления. Выражение инициализации должно вычисляться на этапе компиляции.

### 2.2. Объявление переменной
Переменные могут быть изменяемыми (`var`) или неизменяемыми (`let`).
- `var`: значение может быть изменено оператором присваивания. Инициализация не обязательна (но использование до инициализации запрещено семантикой).
- `let`: значение присваивается один раз при объявлении и не может быть изменено впоследствии.

### 2.3. Объявление функции
- Параметры передаются по значению.
- Возвращаемый тип указывается после списка параметров. Если функция ничего не возвращает, используется тип `Void`.
- Тело функции представляет собой блок кода.

## 3. Инструкции

Инструкции описывают действия, выполняемые программой. Каждая инструкция завершается точкой с запятой `;`, за исключением блоков кода.

### 3.1. Блок кода
Блок кода заключается в фигурные скобки `{ }` и группирует последовательность инструкций. Блок создаёт новую локальную область видимости.

### 3.2. Присваивание
Оператор присваивания `=` записывает значение выражения в ранее объявленную изменяемую переменную. Левая часть должна быть идентификатором переменной.

### 3.3. Условный оператор
Конструкция `if` позволяет выполнить блок кода в зависимости от условия.
- Условие должно быть выражением логического типа (`Bool`).
- Опциональная ветвь `else` выполняется, если условие ложно.

### 3.4. Циклы
Язык поддерживает два типа циклов:
- `while`: выполняет блок, пока условие истинно. Проверка условия происходит перед каждой итерацией.
- `for`: цикл со счётчиком. Состоит из трёх частей: инициализация, условие продолжения, шаг итерации. Все части опциональны.

### 3.5. Возврат из функции
Инструкция `return` завершает выполнение текущей функции.
- Если тип функции не `Void`, `return` должен содержать выражение.
- Если тип функции `Void`, выражение в `return` не требуется.

### 3.6. Ввод и вывод
- `print(expression)`: вычисляет выражение и выводит его строковое представление в стандартный поток вывода.
- `input(variable)`: читает значение из стандартного потока ввода и присваивает его указанной переменной. Тип вводимого значения должен соответствовать типу переменной.

## 4. Выражения и операторы

Выражения строятся из литералов, идентификаторов, вызовов функций и операторов. Порядок вычисления определяется приоритетом и ассоциативностью операторов.

### 4.1. Тернарный условный оператор

Тернарный оператор `?:` позволяет записать условное выражение в компактной форме.

**Синтаксис:**

condition ? true_expression : false_expression

**Правила:**
- `condition` должно иметь тип `Bool`
- Если `condition` равно `true`, вычисляется и возвращается `true_expression`
- Если `condition` равно `false`, вычисляется и возвращается `false_expression`
- Используется **ленивое вычисление**: вычисляется только одна из ветвей
- Оператор **правоассоциативен**: `a ? b : c ? d : e` эквивалентно `a ? b : (c ? d : e)`
- Типы `true_expression` и `false_expression` должны быть совместимы

### 4.2. Приоритет операторов

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

### 4.2. Особенности вычисления
- **Скобки** `()` имеют наивысший приоритет и могут использоваться для явного указания порядка вычислений.
- **Short-circuit evaluation**: операторы `&&` и `||` не вычисляют правый операнд, если результат известен по левому.

## 5. Полная EBNF-грамматика

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
                   | "(" , expression , ")" ;
function-call = identifier , "(" , [ argument-list ] , ")" ;
argument-list = expression , { "," , expression } ;

(* 5. Литералы *)
literal = number-literal | string-literal | boolean-literal ;

number-literal = [ sign ] , integer-part , [ fractional-part ] , [ exponent ] ;

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