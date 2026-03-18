//FizzBuzz — читает число за числом в цикле и далее печатает:
//“Fizz”, если число делится на 3
//“Buzz”, если делится на 5
//“FizzBuzz”, если число делится и на 3, и на 5
//Само число в остальных случаях

const Zero: Int = 0;
const Three: Int = 3;
const Five: Int = 5;
const Fifteen: Int = 15;

func main(): Void {
    var number: Int;
    
    print("Программа FizzBuzz\n");
    print("Вводите числа (0 для выхода):\n");
    
    print("> ");
    input(number);
    
    while (number != Zero) {
        if (number % Fifteen == Zero) {
            print("FizzBuzz\n");
        }
        else if (number % Three == Zero) {
            print("Fizz\n");
        }
        else if (number % Five == Zero) {
            print("Buzz\n");
        }
        else {
            print(number);
            print("\n");
        }
        
        print("> ");
        input(number);
    }   
    print("Программа завершена\n");
}

