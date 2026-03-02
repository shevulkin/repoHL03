//ДЗ 2. Система управління космічними вантажами
public struct Coordinates
{
    public double X;
    public double Y;
    public double Z;
    public Coordinates(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public double GetDistance(Coordinates other)
    {
        double deltaX = other.X - X;
        double deltaY = other.Y - Y;
        double deltaZ = other.Z - Z;

        // Формула відстані у 3D просторі: √((x2 - x1)² + (y2 - y1)² + (z2 - z1)²)
        return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
    }
}

public record CargoManifest(string CargoName, double Weight, string Category);


class SpaceShip
{
    private double _fuel;
    private List<CargoManifest> _cargoList;
    public string Name { get; }
    public Coordinates CurrentPosition { get; private set; }


    public int MaxCapacity { get; set; }

    //Головний конструктор (ім’я та вантажопідйомність).
    public SpaceShip(string name, int maxCapacity)
    {
        Name = name;
        MaxCapacity = maxCapacity;
        _fuel = 100;
        _cargoList = new List<CargoManifest>();
    }

    //Конструктор, що викликає головний через this (ланцюжок конструкторів).
    public SpaceShip() : this("???", 25) { }

    //додає вантаж, якщо його вага не перевищує ліміт.
    public void AddCargo(CargoManifest item)        
    {
        double currentWeight = 0;
        foreach (var cargo in _cargoList)
        {
            currentWeight += cargo.Weight;
        }

        if (currentWeight + item.Weight <= MaxCapacity)
        {
            _cargoList.Add(item);
            Console.WriteLine($"Вантаж додано успішно.");
        }
        else
        {
            Console.WriteLine($"Помилка: Перевищено ліміт вантажопідйомності.");
        }
    }

    //розраховує відстань, витрачає паливо (наприклад, 1 одиниця на 10 одиниць відстані) та оновлює CurrentPosition.
    public void FlyTo(Coordinates newPosition)
    {
        double distance = CurrentPosition.GetDistance(newPosition);
        double fuelNeeded = distance / 10;
        if (fuelNeeded <= _fuel)
        {
            _fuel -= fuelNeeded;
            CurrentPosition = newPosition;
            Console.WriteLine($"Полетіли до нової позиції. Залишок палива: {_fuel}");
        }
        else
        {
            Console.WriteLine($"Недостатньо палива для польоту. Потрібно: {fuelNeeded}, але є: {_fuel}");
        }
    }

    public void PrintCargoList()
    {
        Console.WriteLine("Список вантажів на борту:");
        foreach (var cargo in _cargoList)
        {
            Console.WriteLine($"- {cargo.CargoName} (Вага: {cargo.Weight}, Категорія: {cargo.Category})");
        }
    }



}

class Program
{


    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        //Створіть декілька об'єктів вантажу (Record). Спробуйте скопіювати один за допомогою ключового слова with, змінивши лише назву.
        var cargo1 = new CargoManifest("wax", 10, "Product");
        var cargo2 = new CargoManifest("Carnika", 20, "Bees");
        var cargo3 = cargo2 with { CargoName = "Karpatka" };

        //Створіть екземпляр корабля.
        var spaceship1 = new SpaceShip("Enterprise", 50);
        //Додайте вантаж до корабля, використовуючи створені рекорди.
        spaceship1.AddCargo(cargo1);
        spaceship1.AddCargo(cargo2);
        spaceship1.AddCargo(cargo3);
        //Спробуйте додати вантаж, який перевищує ліміт, і обробіть цю ситуацію.
        spaceship1.AddCargo(cargo3);
        //Задайте початкові координати структурою та відправте корабель у політ до нової точки.
        spaceship1.FlyTo(new Coordinates(70, 10, 300));

        //Виведіть звіт: назву корабля, його поточні координати та список назв вантажів на борту.
        Console.WriteLine($"Name = {spaceship1.Name.ToString()}");
        Console.WriteLine($"координати x = {spaceship1.CurrentPosition.X}");
        Console.WriteLine($"координати y = {spaceship1.CurrentPosition.Y}");
        Console.WriteLine($"координати z = {spaceship1.CurrentPosition.Z}");
        spaceship1.PrintCargoList();


    }
}