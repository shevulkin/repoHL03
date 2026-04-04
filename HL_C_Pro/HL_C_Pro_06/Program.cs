/*
Вам потрібно розробити бекенд-модуль для системи, що аналізує дії студентів під час курсу. Система повинна працювати максимально швидко, оскільки даних дуже багато.

Технічні вимоги:
Унікальні відвідувачі (HashSet):
Створіть клас Student з полями Id (int) та FullName (string).
Обов'язково перевизначте Equals() та GetHashCode() так, щоб два студенти з однаковим Id вважалися одним і тим самим об'єктом (використовуйте HashCode.Combine).
Реалізуйте механізм реєстрації входу на лекцію. Використовуйте HashSet<Student>, щоб гарантувати, що навіть якщо студент випадково відмітився двічі, у списку він залишиться один раз.

Швидкий пошук профілю (Dictionary):
Створіть «Базу профілів» у вигляді Dictionary<int, Student>, де ключем є Id студента.
Напишіть метод, який миттєво ($O(1)$) повертає повне ім'я студента за його номером. Поясніть у коментарях, чому це швидше за List.FirstOrNull().
Хронологія оцінок (SortedDictionary):
Кожен студент має свій журнал оцінок. Використовуйте SortedDictionary<DateTime, int>, де ключ — дата і час роботи, а значення — бал.
Завдяки цій колекції реалізуйте метод, який виводить історію оцінок студента суворо за часом (від найпершої до останньої).

Експеримент «Загублений об’єкт» (Problem Solving):
Додайте студента в HashSet.
Змініть його Id прямо в об'єкті (якщо поле дозволяє зміну).
Спробуйте перевірити Contains() для цього студента. Поясніть у звіті, чому результат false, хоча об'єкт фізично є в пам'яті (відсилка до слайда про незмінність ключа).
 */

// --- Точка входу ---
using System.Text;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.OutputEncoding = Encoding.UTF8;
        // Створюємо студентів один раз — обидві колекції будуть мати посилання на ті самі об'єкти
        var students = new List<Student>
        {
            new() { Id = 1, FullName = "Іван Петренко" },
            new() { Id = 2, FullName = "Марія Коваль" },
            new() { Id = 3, FullName = "Олег Бондаренко" },
        };
        // --- 1. Унікальні відвідувачі (HashSet) ---
        Console.WriteLine("=== HashSet: Реєстрація на лекцію ===");
        var lecture = new LectureAttendance();
        foreach (var s in students) lecture.Register(s);
        lecture.Register(students[0]); // дублікат — той самий об'єкт
        Console.WriteLine($"Унікальних відвідувачів: {lecture.Count}\n");

        // --- 2. Швидкий пошук профілю (Dictionary) ---
        Console.WriteLine("=== Dictionary: База профілів ===");
        var db = new StudentProfileDatabase();
        foreach (var s in students) db.Add(s); // ті самі об'єкти, без дублювання даних
        Console.WriteLine(db.GetFullName(2));   // Марія Коваль
        Console.WriteLine(db.GetFullName(99));  // Студента не знайдено

        // --- 3. Хронологія оцінок (SortedDictionary) ---
        Console.WriteLine("\n=== SortedDictionary: Журнал оцінок ===");
        var journal = new GradeJournal();
        // Додаємо оцінки не в хронологічному порядку — SortedDictionary сортує сам
        journal.AddGrade(new DateTime(2025, 3, 15, 10, 0, 0), 85);
        journal.AddGrade(new DateTime(2025, 1, 10, 9, 0, 0), 72);
        journal.AddGrade(new DateTime(2025, 5, 20, 14, 0, 0), 91);
        journal.AddGrade(new DateTime(2025, 2, 28, 11, 0, 0), 60);
        journal.PrintHistory();

        // --- 4. Експеримент «Загублений об'єкт» ---
        Console.WriteLine("\n=== Експеримент: Загублений об'єкт ===");
        var set = new HashSet<Student>();
        var student = new Student { Id = 10, FullName = "Тест Тестенко" };
        set.Add(student);
        Console.WriteLine($"Додано: {student}");
        Console.WriteLine($"Contains до зміни Id: {set.Contains(student)}"); // true
        // Змінюємо Id — хеш об'єкта тепер інший, але комірка в HashSet стара
        student.Id = 999;
        // HashSet шукає по хешу від нового Id=999 → порожня комірка → false
        // Хоча об'єкт фізично є в пам'яті та в колекції!
        // Це відбувається тому що ключ (хеш) змінився після додавання —
        // HashSet зберігає об'єкт у комірці за старим хешем (Id=10),
        // а шукає у комірці за новим хешем (Id=999) → не знаходить
        Console.WriteLine($"Contains після зміни Id на 999: {set.Contains(student)}"); // false
    }
}

internal class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    // Два студенти вважаються однаковими якщо мають однаковий Id
    public override bool Equals(object? obj) =>
        obj is Student other && Id == other.Id;
    // HashCode.Combine генерує хеш на основі одного або більше полів
    //Якщо не перевизначити GetHashCode() то повертатиме хеш посилання в пам'яті. 
    public override int GetHashCode() => HashCode.Combine(Id);
    public override string ToString() => $"[{Id}] {FullName}";
}

internal class LectureAttendance
{
    // HashSet гарантує унікальність через Equals/GetHashCode:
    // при спробі додати дублікат — ігнорує його
    private readonly HashSet<Student> _attendees = new();
    public void Register(Student student)
    {
        // Add повертає false якщо студент вже є — дублікат мовчки ігнорується
        bool added = _attendees.Add(student);
        Console.WriteLine(added
            ? $"  Зареєстровано: {student}"
            : $"  Вже зареєстровано (дублікат): {student}");
    }
    public int Count => _attendees.Count;
}

internal class StudentProfileDatabase
{
    // Dictionary забезпечує O(1) доступ до профілю за Id через хешування
    private readonly Dictionary<int, Student> _profiles = new();
    public void Add(Student student) => _profiles[student.Id] = student;
    public string GetFullName(int id)
    {
        // Dictionary — це хеш-таблиця. Пошук працює так:
        //   1. Обчислюємо хеш ключа:  hash = id.GetHashCode()
        //   2. Знаходимо комірку:      index = hash % tableSize
        //   3. Читаємо значення з неї: O(1) — одна операція незалежно від кількості елементів
        //
        // List.FirstOrDefault(s => s.Id == id) — O(n):
        //   Перебирає елементи по одному: s[0], s[1], s[2]... поки не знайде потрібний.
        //   При 1 000 000 студентів у гіршому випадку — 1 000 000 порівнянь.
        //   Dictionary при 1 000 000 елементів — все одно 1 операція.
        if (_profiles.TryGetValue(id, out Student? student))
            return student.FullName;
        return "Студента не знайдено";
    }
}

internal class GradeJournal
{
    // SortedDictionary автоматично сортує записи за ключем (DateTime) у зростаючому порядку
    // На відміну від Dictionary — порядок вставки не важливий, виведення завжди хронологічне
    private readonly SortedDictionary<DateTime, int> _grades = new();
    public void AddGrade(DateTime date, int score) => _grades[date] = score;
    public void PrintHistory()
    {
        // Ітерація йде суворо від найранішої до найпізнішої дати
        foreach (KeyValuePair<DateTime, int> entry in _grades)
        {
            Console.WriteLine($"  {entry.Key:dd.MM.yyyy HH:mm} → {entry.Value} балів");
        }
    }
}