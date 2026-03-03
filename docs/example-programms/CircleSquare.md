// CircleSquare - Читает радиус круга и печатает его площадь

const Pi: Float = 3.14159;

func calculateArea(radius: Float): Float {
    return Pi * radius ** 2.0;
}

func main(): Void {
    var radius: Float;          
    print("Введите радиус круга: ");
    input(radius);              
    
    let area: Float = calculateArea(radius);
    
    print("Площадь круга с радиусом ");
    print(radius);
    print(" равна ");
    print(area);
    print("\n");
}