# Интеграционные тесты интерпретатора (TLPMinion)

---

## Программы из файлов (`Programs/ProgramsTest.cs`)

- [x] Минимальное выражение без вывода (`smoke.minion`)
- [x] Сценарий `print_literal.minion`

---

## Модель запуска (`EntryPointTest.cs`)

- [x] Результат `MinionInterpreter.Execute` — значение последнего выражения области (проверка `Value`, не только вывод)
- [x] `ExitCode == 0` при успешном прогоне (`ProgramsTest`)
- [x] Ошибка парсинга / семантики / выполнения: сообщение и код завершения хоста
---

## `print` и `input` (`BuiltinFunctionsTest.cs`)

### Вывод

- [x] `print` целого литерала (`print_literal.minion`)
- [x] Несколько вызовов `print` в одной программе (`print_literal.minion`)
- [x] `print` для `Float`

### Ввод

- [x] `input` в `var` типа `Int` или `Float`
- [x] Некорректное слово при `input` → ошибка выполнения
- [x] Конец очереди в `FakeEnvironment` при `input` → согласованное исключение

### Негативы

- [x] `input` в `let` или `const` → ошибка семантики

---

## Арифметические выражения (`ArithmeticExpressionsTest.cs`)

Операнды `+`, `-`, `*`, `/` — оба `Int` или оба `Float`; `%` — только `Int`; `**` — только `Float`; унарные `+` / `-` — для чисел.

### Целые числа

- [x] Сложение, вычитание, деление
- [x] Умножение (`print_literal.minion`: `x * y` до и после присваивания `x`)
- [x] Приоритет (умножение перед сложением) и скобки
- [x] Остаток `%`
- [x] Деление на ноль для `Int`
- [x] Левоассоциативность `-`, `/` и смешанных цепочек

### Вещественные числа

- [x] `+`, `-`, `*`, `/` для двух `Float`
- [x] Степень `**` и правоассоциативность
- [x] Сочетание степени и унарного `-`
- [x] Деление на ноль для `Float` (без исключения)

### Унарные операторы

- [x] Унарный `+` и `-` для `Int` и `Float`
- [x] Унарный `-` вместе с бинарными операторами

### Негативы

- [x] `Int` и `Float` в одной бинарной операции (`1 + 1.0;`)
- [x] `%` с операндом `Float` (`1.0 % 2.0;`)
- [x] `**` с операндом `Int` (`2 ** 3;`)
- [x] Незакрытая скобка (`(1 + 2;` в `Reject_invalid_syntax_expressions`)

---

## Переменные и присваивание (`VariablesTest.cs`)

`const`, `let`, `var`; типы: `Int`, `Float`, `Void`; область файла и вложенные блоки `{ }`.

### Объявления

- [x] `var x: Int = …` и использование (`print_literal.minion`)
- [x] `let y: Int = …` и использование (`print_literal.minion`)
- [x] `var x: Float;` с последующим присваиванием (`Var_float_without_explicit_initializer_then_assign_and_print`)
- [x] `const` с литералом `Int` (`Const_int_literal_and_print`)

### Присваивание

- [x] Присваивание в `var` того же типа (`print_literal.minion`: `x = 7;`)
- [x] Присваивание в `let` или `const` → ошибка семантики

### Области и затенение

- [x] Вложенный блок с новым объявлением (`Inner_block_shadows_outer_name`)
- [x] Shadowing и восстановление после блока (`Inner_block_shadows_outer_name`)
- [x] Присваивание во внешнюю переменную из внутреннего блока без повторного объявления (`Assign_outer_var_from_inner_block_without_redeclaration`)

### Негативы

- [x] Повторное объявление в той же области (`Duplicate_declaration_in_same_scope_throws`)
- [x] Присваивание необъявленному идентификатору (`Assignment_to_unknown_identifier_throws`)
- [x] Несовпадение типа при инициализации или присваивании (`Init_with_float_literal_to_int_var_throws`, `Assign_float_to_int_var_throws`)
