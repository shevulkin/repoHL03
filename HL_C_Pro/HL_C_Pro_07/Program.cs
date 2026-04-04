using System;
using System.Collections.Generic;
using System.Text;

/*
 * ДЗ 7. Система управління логістичним хабом
Завдання 1: Прийом та відвантаження (Queue & Stack)
Реалізуйте чергу прийому товарів (Queue<string>). Товари додаються в кінець і обробляються по черзі (FIFO).
Реалізуйте зону "завантаження в обмежений простір" (наприклад, вузька вантажівка), де товари вивантажуються в порядку, зворотному до завантаження (LIFO — Stack<string>).

Завдання 2: Інвентаризація та пошук (Dictionary & SortedList)
Створіть головний каталог товарів, де ключем є унікальний штрих-код (int), а значенням — назва товару (Dictionary<int, string>).
Створіть реєстр "VIP-клієнтів" за їхнім рейтингом, де список має бути завжди відсортованим за ID клієнта для швидкого звіту (SortedList<int, string>).

Завдання 3: Історія маніпуляцій (LinkedList)
Використовуйте LinkedList<string> для запису переміщень конкретного коштовного товару по складу.
Реалізуйте можливість швидко додати запис "в середину ланцюжка"

Завдання 4: Порівняльний аналіз (List vs ArrayList)
Напишіть невеликий метод-тест: створіть ArrayList та List<int>.
Додайте в них 1 000 000 цілих чисел.
Заміряйте час виконання та обсяг пам'яті за допомогою Stopwatch. Виведіть результат у консоль
*/

/// <summary>
/// Главний клас програми. Демонструє роботу різних колекцій для логістичного хаба.
/// </summary>
class Program
{
    static void Main()
    {
        // Очистка консолі та встановлення кодування UTF-8 для коректного відображення українських символів
        Console.Clear();
        Console.OutputEncoding = Encoding.UTF8;

        // ========== ЗАВДАННЯ 1: QUEUE & STACK ==========

        // --- Частина 1а: Queue (FIFO - First In First Out) ---
        // Черга прийому товарів: першим доданий = першим оброблений
        Queue<string> intakeQueue = new Queue<string>();

        // Методи Queue<T>:
        // Enqueue (Поставити в чергу): Додає елемент у кінець черги.
        // Dequeue (Витягти з черги): Бере елемент із початку, повертає його та видаляє з колекції.
        // Peek (Подивитися): Дозволяє побачити перший елемент у черзі без видалення.

        // Додаємо товари у чергу прийому
        intakeQueue.Enqueue("Коробка №1 (Мед)");
        intakeQueue.Enqueue("Коробка №2 (Віск)");
        intakeQueue.Enqueue("Коробка №3 (Обніжжя)");

        Console.WriteLine("--- Обробка черги прийому (FIFO) ---");
        // Обробляємо товари у порядку їх прибуття
        while (intakeQueue.Count > 0)
        {
            // Витягуємо перший доданий елемент (видаляємо з черги)
            string currentItem = intakeQueue.Dequeue();
            Console.WriteLine($"Прийнято на склад: {currentItem}");
        }

        Console.WriteLine();

        // --- Частина 1(Б): Stack (LIFO - Last In First Out) ---
        // Стек для вузької вантажівки: останній завантажений = першим вивантажений
        Stack<string> narrowTruckStack = new Stack<string>();

        Console.WriteLine("--- Завантаження товарів у вузьку вантажівку (LIFO) ---");

        // Завантажуємо товари у стек (Push додає елемент на вершину)
        narrowTruckStack.Push("Товар A (Книги)");
        narrowTruckStack.Push("Товар B (Посуд)");
        narrowTruckStack.Push("Товар C (Хрупке - на виході!)");

        Console.WriteLine("Порядок завантаження товарів у стек:");
        Console.WriteLine($"  Push 1: 'Товар A (Книги)'");
        Console.WriteLine($"  Push 2: 'Товар B (Посуд)'");
        Console.WriteLine($"  Push 3: 'Товар C (Хрупке - на виході!)'");

        Console.WriteLine("Усі товари у стеці (без видалення):");
        foreach (var item in narrowTruckStack)
        {
            Console.WriteLine($"  {item}");
        }


        Console.WriteLine("\nВивантаження товарів (у зворотному порядку):");
        // Вивантажуємо товари зі стека (Pop видаляє з вершини)
        while (narrowTruckStack.Count > 0)
        {
            string item = narrowTruckStack.Pop();
            Console.WriteLine($"  Вивантажено: {item}");
        }

        Console.WriteLine();

        // ========== ЗАВДАННЯ 2: DICTIONARY & SORTEDLIST ==========

        // --- Частина 2а: Dictionary (неупорядкована колекція ключ-значення) ---
        // Головний каталог товарів з унікальними штрих-кодами
        Dictionary<int, string> productCatalog = new Dictionary<int, string>()
        {
            // Ключ = штрих-код (int), Значення = назва товару (string)

            { 101, "Мед акацієвий" },
            { 102, "Віск бджільний" },
            { 103, "Прополіс" },
            { 104, "Обніжжя" }
        };

        Console.WriteLine("--- Каталог товарів (за штрих-кодом) ---");
        // Перебираємо пари "ключ-значення" з Dictionary
        foreach (var product in productCatalog)
        {
            Console.WriteLine($"Код {product.Key}: {product.Value}");
        }

        Console.WriteLine();

        // --- Частина 2б: SortedList (упорядкована колекція ключ-значення) ---
        // Реєстр VIP-клієнтів, які автоматично сортуються за ID для швидкого звіту
        SortedList<int, string> vipClients = new SortedList<int, string>()
        {
            { 5, "Клієнт E (Рейтинг 5)" },   // Додаємо в довільному порядку
            { 1, "Клієнт A (Рейтинг 1)" },
            { 3, "Клієнт C (Рейтинг 3)" },
            { 2, "Клієнт B (Рейтинг 2)" },
            { 4, "Клієнт D (Рейтинг 4)" }
        };

        Console.WriteLine("--- Реєстр VIP-клієнтів (відсортовані за ID) ---");
        // SortedList автоматично зберігає елементи в упорядкованому вигляді
        foreach (var client in vipClients)
        {
            Console.WriteLine($"ID {client.Key}: {client.Value}");
        }

        Console.WriteLine();

        // ========== ЗАВДАННЯ 3: LINKEDLIST ==========

        // Ланцюг для запису історії переміщень дорогоцінного товару по складу
        LinkedList<string> itemHistory = new LinkedList<string>();

        // AddLast: додаємо елемент в кінець ланцюга
        itemHistory.AddLast("2024-01-15 10:30 - Товар прибув на склад, зона A1");
        itemHistory.AddLast("2024-01-15 11:45 - Переміщено у климатичну зону B2");
        itemHistory.AddLast("2024-01-15 14:20 - Переміщено у безпечне сховище C3");

        // Додавання запису В СЕРЕДИНУ ланцюга (велика перевага LinkedList)
        // First.Next вказує на другий вузол ланцюга
        var middleNode = itemHistory.First.Next;
        // AddAfter вставляє новий елемент відразу після вказаного вузла
        itemHistory.AddAfter(middleNode, "2024-01-15 11:00 - Проведена інспекція якості");

        Console.WriteLine("--- Історія переміщень коштовного товару (Дорогоцінна люцерна) ---");
        // Перебираємо ланцюг у порядку від першого до останнього вузла
        foreach (var record in itemHistory)
        {
            Console.WriteLine($"  {record}");
        }

        Console.WriteLine();

        // ========== ЗАВДАННЯ 4: ПОРІВНЯЛЬНИЙ АНАЛІЗ ==========

        // Тест продуктивності між ArrayList (неоптимізованим) та List<T> (оптимізованим дженериком)
        Console.WriteLine("--- Порівняння List<int> vs ArrayList (1,000,000 елементів) ---");
        CompareCollections();
    }

    /// <summary>
    /// Метод для порівняння продуктивності ArrayList та List{int}.
    /// Додає 1 мільйон елементів до обох колекцій та вимірює час виконання.
    /// </summary>
    static void CompareCollections()
    {
        // Створюємо лічильник для вимірювання часу виконання
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        const int elementCount = 1_000_000;

        // ========== Тест 1: ArrayList (неоптимізована, без типізації) ==========
        // ArrayList - старовинна колекція, що зберігає об'єкти як Object (без типобезпеки)

        sw.Start();
        System.Collections.ArrayList arrayList = new System.Collections.ArrayList();
        // Додаємо 1 мільйон чисел
        for (int i = 0; i < elementCount; i++)
        {
            arrayList.Add(i);  // Boxing! Кожна int перетворюється на object
        }
        sw.Stop();
        long arrayListTime = sw.ElapsedMilliseconds;

        // Скидаємо лічильник для наступного тесту
        sw.Reset();

        // ========== Тест 2: List<int> (оптимізована, з типізацією) ==========
        // List<T> - сучасна дженерик-колекція, збереже значення безпосередньо без boxing

        sw.Start();
        List<int> list = new List<int>();
        // Додаємо 1 мільйон чисел
        for (int i = 0; i < elementCount; i++)
        {
            list.Add(i);  // Прямо додаємо без перетворення типів
        }
        sw.Stop();
        long listTime = sw.ElapsedMilliseconds;

        // ========== Вивід результатів ==========

        Console.WriteLine($"ArrayList:");
        Console.WriteLine($"  Час виконання: {arrayListTime} мс");
        Console.WriteLine($"  Характеристика: неоптимізована, з boxing/unboxing");

        Console.WriteLine($"\nList<int>:");
        Console.WriteLine($"  Час виконання: {listTime} мс");
        Console.WriteLine($"  Характеристика: оптимізована дженерик-колекція");

        // Розраховуємо різницю та виводимо висновок
        string conclusion = arrayListTime > listTime ? 
            $"швидше на {arrayListTime - listTime} мс" : 
            "однакова або швидша";
        Console.WriteLine($"\nВисновок: List<int> {conclusion}");
        Console.WriteLine($"Причина: List<T> не вимагає boxing, вищча типобезпека");

      //  */
    }
}