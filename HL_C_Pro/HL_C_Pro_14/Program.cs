

using HL_C_Pro_14.Models;

using var db = new PostgresContext();


/*
 * 
 * Завдання 1: Вибірка та сортування (SELECT, WHERE, ORDER BY)
Напишіть запит, який виведе назви (Title) та ціни (Price) усіх книг, які коштують більше 300 грн,
і відсортуйте їх від найдорожчої до найдешевшої.
 * 
 */

var books = db.Books
    .Where(b => b.Price > 300)
    .OrderByDescending(b => b.Price)
    .Select(b => new { b.Title, b.Price })
    .ToList();

foreach (var b in books)
{
    Console.WriteLine($"{b.Title} - {b.Price} грн");
}

/*
 * Завдання 2: Додавання даних (INSERT)
Додайте в таблицю Categories нову категорію «Програмування», а в таблицю Books — книгу з назвою «Clean Code», ціною 600 грн та відповідним CategoryId.
 */

var category = new Category
{
    Categoryname = "Програмування"
};
//db.Categories.Add(category);
//db.SaveChanges();

var book = new Book
{
    Title = "Clean Code",
    Price = 600,
    Categoryid = category.Id,
    Isdeleted = new System.Collections.BitArray(1, false)
};
//db.Books.Add(book);
//db.SaveChanges();

/*
 * Завдання 3: Оновлення даних (UPDATE)
Змініть ціну книги з назвою «Clean Code» на 750 грн. Важливо: не забудьте використати умову WHERE, щоб не оновити всі записи в таблиці.
 */

var book3 = db.Books.FirstOrDefault(b => b.Title == "Clean Code");

if (book3 != null)
{
    book3.Price = 750;
    db.SaveChanges();
    Console.WriteLine("SUCCESS");
}
else
{
    Console.WriteLine("FAIL");
}

/*
 * Завдання 4: Об'єднання таблиць (INNER JOIN)
Напишіть запит, який виведе список усіх книг (Title) разом із назвою їхньої категорії (CategoryName). Використовуйте INNER JOIN, щоб отримати лише ті книги, які мають прив'язану категорію.
 */
//v1
 var booksWithCategories = db.Books
    .Join(db.Categories,
        b => b.Categoryid,
        c => c.Id,
        (b, c) => new { b.Title, CategoryName = c.Categoryname })
    .ToList();

foreach (var bc in booksWithCategories)
{
    Console.WriteLine($"{bc.Title} - {bc.CategoryName}");
}

//v2
var booksWithCategories2 = db.Books
    .Select(b => new { b.Title, CategoryName = b.Category.Categoryname })
    .ToList();

foreach (var bc in booksWithCategories2)
{
    Console.WriteLine($"{bc.Title} - {bc.CategoryName}");
}

/*
 * 
 * Завдання 5: Групування та агрегація (GROUP BY)
Порахуйте кількість книг у кожній категорії. Результат має містити CategoryId та колонку TotalBooks із кількістю записів.
 */

var bookCounts = db.Books
    .GroupBy(b => b.Categoryid)
    .Select(g => new { CategoryId = g.Key, TotalBooks = g.Count() })
    .ToList();

foreach (var bc in bookCounts)
{
    Console.WriteLine($"CategoryId: {bc.CategoryId} - TotalBooks: {bc.TotalBooks}");
}

/*
 * Завдання 6: Видалення даних (Soft Delete)
Замість повного видалення книги з Id = 5 за допомогою команди DELETE, виконайте «м’яке видалення» (Soft Delete). Напишіть запит, який просто змінить статус IsDeleted на 1 для цього запису
 */
var bookToDelete = db.Books.FirstOrDefault(b => b.Id == 5);

if (bookToDelete != null)
{
    bookToDelete.Isdeleted = new System.Collections.BitArray(1, true);
    db.SaveChanges();
    Console.WriteLine("SUCCESS");
}
else
{
    Console.WriteLine("FAIL");
}