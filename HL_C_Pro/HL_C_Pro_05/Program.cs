using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


/*
 * ДЗ 6. Автоматизована система обробки банківських транзакцій

 1. Робота з Атрибутами та Моделями
Створіть клас Transaction, який описує грошовий переказ. Додайте до нього наступні властивості: Id, Amount, Currency, SenderName та SecretAuthCode.

Використовуючи знання про атрибути:

Створіть власний атрибут [TransactionInfo(string author, string version)]. Позначте ним клас Transaction, вказавши себе як автора.
Використовуйте стандартні атрибути серіалізації (наприклад, з System.Text.Json):
Поле SenderName має серіалізуватися в JSON як sender_full_name.
Поле SecretAuthCode має бути повністю ігнорованим під час серіалізації (приховано).
2. Реалізація "Інспектора типів" (Рефлексія)
Напишіть сервіс або метод, який за допомогою рефлексії:

Отримує об'єкт будь-якого типу.
Виводить у консоль інформацію про наявність атрибута [TransactionInfo]: ім'я автора та версію.
Динамічно виводить список усіх публічних властивостей об'єкта та їхніх поточних значень.
(Додатково) Спробуйте за допомогою рефлексії змінити значення приватного поля в об'єкті.
3. Серіалізація та збереження стану
Реалізуйте логіку збереження та відновлення даних:

Створіть декілька об'єктів Transaction.
Серіалізуйте їх у формат JSON та збережіть у файл (або виведіть у консоль).
Виконайте десеріалізацію з JSON-рядка назад в об'єкт та переконайтеся, що дані відновлені коректно, а секретне поле SecretAuthCode залишилося порожнім (null або за замовчуванням)
 * 
 */


internal class Program
{
    static void Main(string[] args)
    {
        Console.Clear();
        Console.OutputEncoding = Encoding.UTF8;
        
        var transaction = new Transaction
        {
            Id = 1,
            Amount = 1500.75m,
            Currency = "UAH",
            SenderName = "Іван Петренко",
            SecretAuthCode = "TOP_SECRET_123"
        };

        // Інспектуємо об'єкт Transaction та його атрибути
        TypeInspector.Inspect(transaction);

        // Демонструємо зміну приватного поля через рефлексію
        Console.WriteLine($"\nSecretAuthCode до зміни: {transaction.SecretAuthCode}");

        // Ім'я поля для автосвойства: <НазваСвойства>k__BackingField
        TypeInspector.SetPrivateField(transaction, "<SecretAuthCode>k__BackingField", "CHANGED_VIA_REFLECTION");

        // Після зміни
        Console.WriteLine($"SecretAuthCode після зміни: {transaction.SecretAuthCode}");

        // 3. Серіалізація та збереження стану
        Console.WriteLine("\n=== Серіалізація ===");

        var transactions = new List<Transaction>
        {
            new() { Id = 1, Amount = 1500.75m, Currency = "UAH", SenderName = "Іван Петренко",  SecretAuthCode = "SECRET_1" },
            new() { Id = 2, Amount = 250.00m,  Currency = "USD", SenderName = "Марія Коваль",   SecretAuthCode = "SECRET_2" },
            new() { Id = 3, Amount = 9999.99m, Currency = "EUR", SenderName = "Олег Бондаренко", SecretAuthCode = "SECRET_3" },
        };

        // Налаштування для гарного форматування JSON
        var options = new JsonSerializerOptions { WriteIndented = true };

        // Серіалізуємо список у JSON-рядок
        string json = JsonSerializer.Serialize(transactions, options);

        // Виводимо заголовок
        Console.WriteLine("JSON:");

        // виводимо сам JSON у консоль
        Console.WriteLine(json);

        // Зберігаємо у файл
        string filePath = "transactions.json";
        File.WriteAllText(filePath, json, Encoding.UTF8);
        Console.WriteLine($"\nЗбережено у файл: {Path.GetFullPath(filePath)}");

        // Десеріалізуємо з файлу назад в об'єкти
        Console.WriteLine("\n=== Десеріалізація ===");
        string jsonFromFile = File.ReadAllText(filePath, Encoding.UTF8);
        List<Transaction>? restored = JsonSerializer.Deserialize<List<Transaction>>(jsonFromFile);
        if (restored is not null)
        {
            foreach (Transaction t in restored)
            {
                // SecretAuthCode має бути null — воно позначено [JsonIgnore]
                Console.WriteLine($"Id={t.Id} | {t.SenderName} | {t.Amount} {t.Currency} | SecretAuthCode={t.SecretAuthCode ?? "null (проігноровано)"}");
            }
        }
    }
}

// Створюємо атрибут з параметрами
//маркувати можна класи та методи
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

internal sealed class TransactionInfoAttribute(string author, string version) : Attribute
{
    public string Author { get; } = author;
    public string Version { get; } = version;
}

// Клас Transaction маркуємо атрибутом
[TransactionInfo(author: "Євген", version: "1.0")]

internal class Transaction
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public required string Currency { get; set; }

    // Використовуємо атрибути для налаштування JSON-серіалізації
    [JsonPropertyName("sender_full_name")]

    public required string SenderName { get; set; }

    // це поле не повинно серіалізуватися в JSON
    [JsonIgnore]

    public string? SecretAuthCode { get; set; }

}

//Рефлексія
// Клас для інспекції типів та роботи з атрибутами

internal static class TypeInspector
{

    public static void Inspect(object obj)
    {
        Type type = obj.GetType();
        Console.WriteLine($"=== Інспекція типу: {type.Name} ===");

        // 1. Перевірка атрибута [TransactionInfo]
        var transactionInfo = type.GetCustomAttribute<TransactionInfoAttribute>();
        if (transactionInfo is not null)
        {
            Console.WriteLine($"[TransactionInfo] знайдено:");
            Console.WriteLine($"  Автор:  {transactionInfo.Author}");
            Console.WriteLine($"  Версія: {transactionInfo.Version}");
        }
        else
        {
            Console.WriteLine("[TransactionInfo] не знайдено.");
        }

        // 2. Публічні властивості та їхні значення
        Console.WriteLine("\nПублічні властивості:");

        // Отримуємо всі публічні властивості екземпляра
        // BindingFlags.Public - тільки публічні
        // BindingFlags.Instance - тільки екземплярні (не статичні)
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Виводимо тип, ім'я та поточне значення кожної властивості
        foreach (PropertyInfo prop in properties)
        {
            object? value = prop.GetValue(obj);
            Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name} = {value}");
        }
    }
    public static void SetPrivateField(object obj, string fieldName, object newValue)

    {

        FieldInfo? field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (field is not null)
        {
            field.SetValue(obj, newValue);
        }
        else
        {
            Console.WriteLine($"[Рефлексія] Поле '{fieldName}' не знайдено.");
        }
    }
}