using System.Text;

/*
 Вам потрібно розробити систему для управління товарами на складі.
Кожен склад має обмежену кількість комірок (використовуємо індексатори)
та систему оповіщення про критичні зміни (використовуємо події та лямбда-вирази).

Технічні вимоги
1. Клас Warehouse (Склад)
Внутрішнє сховище: Створіть масив рядків _items фіксованого розміру (наприклад, 10).
Індексатор: Реалізуйте індексатор public string this[int index], який дозволить класти товар у комірку та діставати його.
Подія: Оголосіть подію event Action<int, string> OnItemChanged (використовуючи стандартний делегат Action). Подія має спрацьовувати щоразу, коли в комірку записується нове значення через індексатор.
Параметри події: номер комірки та назва товару.
2. Клас Program (Логіка роботи)
Створення об'єкта: Створіть екземпляр Warehouse.
Підписка на подію: Використовуючи лямбда-вираз, підпишіться на подію OnItemChanged. Лямбда має виводити в консоль повідомлення: "Лог: У комірку №... додано товар: ...".
Фільтрація (Predicate): Створіть список назв товарів, які ви хочете додати. Використовуйте Predicate<string>, щоб перевірити, чи назва товару не є порожньою перед тим, як покласти її у склад.
Обробка даних (Func): Використовуйте Func<string, string>, щоб автоматично перетворювати назву товару у верхній регістр (ToUpper()) перед збереженням.
*/


internal class Program
{
    // Делегат для перевірки валідності назви товару
    private static readonly Predicate<string> isValid = name => !string.IsNullOrWhiteSpace(name);

    // Делегат для нормалізації назви товару
    private static readonly Func<string, string> normalize = name => name.ToUpper();
    static void Main(string[] args)

    {
        Console.OutputEncoding = Encoding.UTF8;
        // Створюємо склад
        var warehouse = new Warehouse();

        // Підписуємося на подію
        warehouse.OnItemChanged += (index, item) =>
            Console.WriteLine($"Лог: У комірку №{index} додано товар: {item}");

        var products = new List<string> { "Мед", "Перга", "", "прополюс", null, "геМогенат" };

        // Додаємо товари, нормалізуємо, та виводимо подію для кожного товару
        for (int i = 0, slot = 0; i < products.Count; i++)
        {
            if (isValid(products[i]))
            {
                warehouse[slot++] = normalize(products[i]);
            }
        }
    }
}

internal class Warehouse

{
    // Комірки для зберігання товарів
    private string[] _items = new string[10];

    // Подія для логування змін у комірках
    public event Action<int, string> OnItemChanged;

    // Індексатор для доступу до комірок
    public string this[int index]
    {
        get => _items[index];

        set
        {
            _items[index] = value;
            OnItemChanged?.Invoke(index, value);
        }
    }
}