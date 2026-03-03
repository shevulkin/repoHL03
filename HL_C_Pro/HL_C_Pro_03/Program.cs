using System.Globalization;

/*
1. Абстракція та Наслідування
Створіть абстрактний клас StationModule.
Поля (Інкапсуляція): Приватне поле _energyConsumption.
Властивості: string Name (init-only), IsRunning (bool).
Конструктор: Встановлює назву та базове споживання енергії.
Абстрактний метод: void PerformDiagnostics().
Похідні класи:
LifeSupportModule — при діагностиці виводить стан кисню.
ScienceModule — додає поле List<string> ResearchProjects.
*/
abstract class StationModule
{
    private readonly double _energyConsumption;
    public string Name { get; init; }
    public bool IsRunning { get; protected set; }
    protected StationModule(string name, double baseEnergyConsumption)
    {
        Name = name;
        _energyConsumption = baseEnergyConsumption;
    }

    /*
    2. Поліморфізм
    У базовому класі StationModule створіть віртуальний метод Start().
    У ScienceModule перевизначте (override) цей метод так, щоб він не лише вмикав модуль,
    а й виводив повідомлення про підготовку мікроскопів.
    */
    public virtual void Start()
    {
        IsRunning = true;
        Console.WriteLine($"[START] {Name,-12} | Energy: {_energyConsumption,6:F1} kW");
    }

    public abstract void PerformDiagnostics();
}

// при діагностиці виводить стан кисню.
class LifeSupportModule : StationModule
{
    public double OxygenLevel { get; set; }

    public LifeSupportModule(string name, double baseEnergyConsumption, double oxygenLevel)
        : base(name, baseEnergyConsumption)
    {
        OxygenLevel = oxygenLevel;
    }

    public override void PerformDiagnostics()
    {
        Console.WriteLine($"[DIAG] LifeSupport | Oxygen: {OxygenLevel,5:F1}%");
    }
}

class ScienceModule : StationModule
{
    public List<string> ResearchProjects { get; } = new();

    public ScienceModule(string name, double baseEnergyConsumption)
        : base(name, baseEnergyConsumption)
    {
    }

    public override void Start()
    {
        // Поліморфізм: розширюємо стандартний запуск модуля
        base.Start();
        Console.WriteLine("[INFO] ScienceLab | Microscopes are being prepared...");
    }

    public override void PerformDiagnostics()
    {
        Console.WriteLine($"[DIAG] ScienceLab | Active projects: {ResearchProjects.Count}");
    }
}

/*

3. Інтерфейси
Створіть інтерфейс IEnergySource (Джерело енергії).
Члени: Метод double GetOutput(), властивість string EnergyType.
Реалізація:
Клас SolarPanel (повертає енергію залежно від "інтенсивності сонця").
Клас NuclearReactor (повертає стабільну велику потужність).
*/
interface IEnergySource
{
    string EnergyType { get; }
    double GetOutput();
}

class SolarPanel : IEnergySource
{
    public string EnergyType => "Solar";
    public double SunIntensity { get; set; }
    public double MaxOutput { get; }

    public SolarPanel(double sunIntensity, double maxOutput = 120.0)
    {
        SunIntensity = Math.Clamp(sunIntensity, 0.0, 1.0);
        MaxOutput = maxOutput;
    }

    public double GetOutput() => MaxOutput * SunIntensity;
}

class NuclearReactor : IEnergySource
{
    public string EnergyType => "Nuclear";
    public double StableOutput { get; }

    public NuclearReactor(double stableOutput = 1500.0)
    {
        StableOutput = stableOutput;
    }

    public double GetOutput() => StableOutput;
}



/*
4. Дженеріки (Generics) та Обмеження
Створіть дженерік-клас StorageUnit<T>, де T — це вантаж.
Обмеження: T має бути посилальним типом і мати конструктор за замовчуванням (where T : class, new()).
Поля: List<T> _items.
Методи: AddItem(T item), T GetItem(int index).
*/

class StorageUnit<T> where T : class, new()
{
    private readonly List<T> _items = new();

    public void AddItem(T item)
    {
        _items.Add(item);
    }

    public T GetItem(int index)
    {
        return _items[index];
    }
}



/*
5. Using та IDisposable
Зробіть клас StationLog (журнал подій), який реалізує IDisposable.
Метод Write(string message) записує події у файл.
Використайте using declaration у Main, щоб гарантувати запис логів на диск після завершення симуляції.
*/
class StationLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    public StationLog(string filePath)
    {
        _writer = new StreamWriter(filePath, append: true);
        _writer.AutoFlush = true;
    }

    public void Write(string message)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(StationLog));
        }

        var timestamp = DateTime.Now.ToString("u", CultureInfo.InvariantCulture);
        _writer.WriteLine($"[{timestamp}] {message}");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _writer.Dispose();
        _disposed = true;
    }
}



// Приклад вантажу для складу
class CargoBox
{
    public string Label { get; set; } = "Unknown cargo";
}

class Program
{
    static void Main()
    {
        Console.WriteLine("==============================================");
        Console.WriteLine(" SPACE STATION MODULES - SIMULATION");
        Console.WriteLine("==============================================\n");

        // Створення модулів станції
        var lifeSupport = new LifeSupportModule("LifeSupport", 320.0, 97.6);
        var science = new ScienceModule("ScienceLab", 460.0);
        science.ResearchProjects.Add("Microgravity Cell Study");
        science.ResearchProjects.Add("Crystal Growth Experiment");

        // Підключення джерел енергії через інтерфейс
        IEnergySource solarPanel = new SolarPanel(sunIntensity: 0.72);
        IEnergySource reactor = new NuclearReactor();

        // Робота з універсальним складом
        var storage = new StorageUnit<CargoBox>();
        storage.AddItem(new CargoBox { Label = "Medical Supplies" });
        storage.AddItem(new CargoBox { Label = "Spectrometer Parts" });

        // using declaration гарантує Dispose наприкінці методу
        using StationLog log = new("station.log");

        Console.WriteLine("--- MODULE STARTUP ---");

        // Запуск та діагностика модулів
        lifeSupport.Start();
        science.Start();

        Console.WriteLine("\n--- DIAGNOSTICS ---");

        lifeSupport.PerformDiagnostics();
        science.PerformDiagnostics();

        Console.WriteLine("\n--- ENERGY SOURCES ---");
        Console.WriteLine($"{solarPanel.EnergyType,-10}: {solarPanel.GetOutput(),7:F1} kW");
        Console.WriteLine($"{reactor.EnergyType,-10}: {reactor.GetOutput(),7:F1} kW");

        var firstCargo = storage.GetItem(0);
        Console.WriteLine("\n--- STORAGE ---");
        Console.WriteLine($"Item #1: {firstCargo.Label}");

        log.Write("Simulation started.");
        log.Write("Diagnostics finished successfully.");
        log.Write("Energy systems are operating in nominal range.");

        Console.WriteLine("\n✅ Simulation complete. Logs written to station.log");
    }
}