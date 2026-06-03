# Список тестов Semantics.UnitTests

## `CompileTimeConstantPassTests.cs`

### Положительные (парсер + `SemanticsChecker`, проверка `CompileTimeValue`)

- [x] Цепочка `Int`: ссылки и `+`, `*`, `-`
- [x] Унарные `+` / `-` для `Int`
- [x] Целочисленное `/` и `%`
- [x] Степень `**` для `Float`
- [x] Унарный `-` для `Float`
- [x] Литерал `Bool` в `const`

### Негативы (парсер + `SemanticsChecker`)

- [x] Деление на ноль и `%` на ноль для `Int`
- [x] Деление `Float` на ноль (`DivFloat`)
- [x] Ссылка вперёд по тексту (ошибка `ResolveNamesPass`)
- [x] Ссылка на не-`const` в инициализаторе
- [x] Переполнение `checked` при сложении `Int`
- [x] Унарный `-` у строки в `const`
- [x] Несовпадение типа `const X: Float = 1`

### Ручная сборка AST (только `CompileTimeConstantPass`)

- [x] Ссылка на `const` до вычисления `CompileTimeValue` — сообщение «должна быть объявлена выше по тексту`
