// Программа IsPrime
// Читает число N и проверяет, является ли оно простым
// Используется оптимизация: проверка делителей до квадратного корня из N

const One: Int = 1;
const Two: Int = 2;
const Zero: Int = 0;

// Функция для вычисления квадратного корня (для Int)
func intSqrt(value: Int): Int {
    // Используем метод Ньютона для целых чисел
    if (value <= One) {
        return value;
    }
    
    var guess: Int = value / Two;
    var prevGuess: Int;
    
    while (true) {
        prevGuess = guess;
        guess = (guess + value / guess) / Two;
        
        // Если значение стабилизировалось или начало расти
        if (guess >= prevGuess) {
            // Возвращаем меньшее из двух, чтобы не превысить корень
            if (guess > value / guess) {
                return prevGuess;
            } else {
                return guess;
            }
        }
    }
}

// Функция проверки числа на простоту
func isPrime(n: Int): Bool {
    if (n < Two) {
        return false;
    }    
 
    if (n == Two) {
        return true;
    }
 
    if (n % Two == Zero) {
        return false;
    }
    
    // Получаем целую часть квадратного корня
    let limit: Int = intSqrt(n);
    
    // Проверяем делители от 3 до sqrt(n) с шагом 2 (только нечетные)
    var divisor: Int = 3;
    while (divisor <= limit) {
        if (n % divisor == Zero) {
            return false;  
        }
        divisor = divisor + Two;  // Шаг 2
    }    
    return true;
}

func main(): Void {
    var n: Int;
    
    print("Введите число для проверки на простоту: ");
    input(n);
    
    if (isPrime(n)) {
        print(n);
        print(" - простое число\n");
    } else {
        print(n);
        print(" - не является простым числом\n");
    }
}