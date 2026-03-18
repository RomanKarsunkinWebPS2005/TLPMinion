// Программа SquareRoot
// Читает действительное число и печатает его квадратный корень
// Печатает ERROR, если корень не является действительным числом (число < 0)

const Zero: Float = 0.0;

func sqrt(value: Float): Float {
    if (value < Zero) {
        return -1.0;  // специальный код ошибки
    }
    
    if (value == Zero) {
        return Zero;
    }
    
    var guess: Float = value / 2.0;
    let epsilon: Float = 0.00001;  // точность вычисления
    
    // Метод Ньютона: guess = (guess + value/guess) / 2
    var diff: Float;
    var newGuess: Float;
    
    newGuess = (guess + value / guess) / 2.0;
    diff = newGuess - guess;
    
    // Используем абсолютное значение для проверки точности
    while ((diff > epsilon) || (-diff > epsilon)) {
        guess = newGuess;
        newGuess = (guess + value / guess) / 2.0;
        diff = newGuess - guess;
    }
    
    return newGuess;
}

func main(): Void {
    var number: Float;
    
    print("Введите число для вычисления квадратного корня: ");
    input(number);
    
    if (number < Zero) {
        print("ERROR\n");
    } else {
        let result: Float = sqrt(number);
        print("Квадратный корень из ");
        print(number);
        print(" равен ");
        print(result);
        print("\n");
    }
}
