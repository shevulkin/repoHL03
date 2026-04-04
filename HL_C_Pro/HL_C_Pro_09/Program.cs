using System.Text;
/*
ДЗ 9. Система обробки транзакцій Crypto-Exchange
//
Етап 1: Боротьба із «Race Condition» (Станом гонитви)
Уявіть, що у вас є спільний рахунок біржі для виплати бонусів. Створіть клас BonusAccount із початковим балансом 0.
//
Завдання: Запустіть 100 окремих задач (Task.Run), кожна з яких 1000 разів додає до балансу 1 грн.
Мета: Спочатку отримати неправильний результат через «Race Condition».
Виправлення: Використайте Interlocked.Increment або lock, щоб гарантувати, що кожен потік коректно оновлює баланс.
//
Етап 2: Протоколювання через Concurrent-колекції
Біржа має записувати кожну транзакцію в історію. Оскільки записів багато і вони йдуть із різних потоків, звичайна List<T> не підійде.
//
Завдання: Використайте ConcurrentQueue<string> для зберігання логів транзакцій.
Мета: Кілька потоків-«трейдерів» мають записувати повідомлення у чергу одночасно, не блокуючи один одного.
//
Етап 3: Обмеження ресурсів (Семафор)
Ваш бот має підключитися до 10 різних API для перевірки курсу, але сервер біржі дозволяє лише 3 одночасні з'єднання, щоб не «покласти» систему.
//
Завдання: Використайте SemaphoreSlim(3), щоб обмежити кількість активних потоків, які одночасно імітують запит до API (через Task.Delay).
//
Етап 4: Розумна черга (BlockingCollection)
Реалізуйте модель «Виробник — Споживач» (Producer-Consumer).
//
Завдання: Використайте BlockingCollection<T> як обгортку для черги транзакцій.
Мета: Один потік постійно генерує нові транзакції, а інший — «засинає», поки даних немає, і миттєво прокидається для обробки, як тільки вони з’являються
*/
//
Console.Clear();
Console.OutputEncoding = Encoding.UTF8;
//
// ── 1а. БЕЗ синхронізації — демонструємо Race Condition ──────────────────────
Console.WriteLine("--- 1а. БЕЗ синхронізації (Race Condition) ---");
// Створюємо спільний рахунок з балансом 0
var unsafeAccount = new BonusAccount();
//
// Масив для збереження 100 задач
var unsafeTasks = new Task[100];
for (int i = 0; i < 100; i++)
{
    // Task.Run запускає код у окремому потоці з пулу потоків
    unsafeTasks[i] = Task.Run(() =>
    {
        for (int j = 0; j < 1000; j++)
            unsafeAccount.AddUnsafe(1m); // НЕ потокобезпечно! потоки перезаписують одне одного
    });
}
// Чекаємо поки ВСІ 100 задач завершаться
await Task.WhenAll(unsafeTasks);
//
// Якщо Race Condition виникла — баланс буде меншим за 100 000
Console.WriteLine($"Очікується : 100 000 UAH");
Console.WriteLine($"Отримано   : {unsafeAccount.Balance:N0} UAH");
Console.WriteLine(unsafeAccount.Balance == 100_000m
    ? "Результат ПРАВИЛЬНИЙ (пощастило — гонитви не виникло)"
    : "Результат НЕПРАВИЛЬНИЙ — Race Condition виявлено!");
//
// ── 1б. З синхронізацією через lock — виправлення ────────────────────────────
// Примітка: Interlocked не підтримує decimal, тому використовуємо lock.
// lock гарантує, що лише ОДИН потік одночасно може змінювати баланс.
Console.WriteLine();
Console.WriteLine("--- 1б. З синхронізацією через lock (виправлення) ---");
//
var safeAccount = new BonusAccount();
//
var safeTasks = new Task[100];
for (int i = 0; i < 100; i++)
{
    safeTasks[i] = Task.Run(() =>
    {
        for (int j = 0; j < 1000; j++)
            // AddSafe всередині використовує lock — інші потоки чекають у черзі
            safeAccount.AddSafe(1m);
    });
}
await Task.WhenAll(safeTasks);
//
// Тепер результат завжди буде точно 100 000
Console.WriteLine($"Очікується : 100 000 UAH");
Console.WriteLine($"Отримано   : {safeAccount.Balance:N0} UAH");
Console.WriteLine(safeAccount.Balance == 100_000m
    ? "Результат ПРАВИЛЬНИЙ — Race Condition усунуто за допомогою lock!"
    : "Результат НЕПРАВИЛЬНИЙ — щось пішло не так.");
//
// ═════════════════════════════════════════════════════════════════════════════
// Етап 2: Протоколювання через Concurrent-колекції
// ConcurrentQueue<string> — потокобезпечна черга без блокувань
// ═════════════════════════════════════════════════════════════════════════════
//
Console.WriteLine();
Console.WriteLine("--- Етап 2: Логування транзакцій через ConcurrentQueue ---");
//
// ConcurrentQueue — потокобезпечна черга: кілька потоків можуть
// одночасно додавати елементи без lock і без втрати даних
var transactionLog = new System.Collections.Concurrent.ConcurrentQueue<string>();
//
// 10 трейдерів, кожен виконує 5 транзакцій одночасно
var traderTasks = new Task[10];
for (int i = 0; i < 10; i++)
{
    // Важливо: копіюємо i в окрему змінну traderId.
    // Без цього всі лямбди захопили б одну змінну i,
    // і до моменту виконання вона вже дорівнювала б 10.
    int traderId = i + 1;
    traderTasks[i] = Task.Run(() =>
    {
        for (int j = 1; j <= 5; j++)
        {
            // D2 — форматування числа з мінімум 2 цифрами (01, 02 ... 10)
            string message = $"[Трейдер {traderId:D2}] Транзакція #{j}: +{j * 100:N0} UAH о {DateTime.Now:HH:mm:ss.fff}";
            // Enqueue — атомарна операція, не потребує lock
            transactionLog.Enqueue(message);
        }
    });
}
//
// Чекаємо завершення всіх трейдерів
await Task.WhenAll(traderTasks);
//
Console.WriteLine($"Всього записів у черзі: {transactionLog.Count} (очікується 50)");
Console.WriteLine();
Console.WriteLine("Перші 10 записів із черги:");
//
int printed = 0;
// TryDequeue: якщо черга порожня — повертає false і цикл зупиняється
// out string? entry — оголошення змінної прямо в умові
while (printed < 10 && transactionLog.TryDequeue(out string? entry))
{
    Console.WriteLine("  " + entry);
    printed++;
}
//
Console.WriteLine($"  ... ще {transactionLog.Count} записів залишилось у черзі.");
//
// ═════════════════════════════════════════════════════════════════════════════
// Етап 3: Обмеження ресурсів (Семафор)
// Сервер біржі дозволяє лише 3 одночасні з'єднання з API курсів валют.
// SemaphoreSlim(3) гарантує, що в будь-який момент активних запитів не > 3.
// ═════════════════════════════════════════════════════════════════════════════
//
Console.WriteLine();
Console.WriteLine("--- Етап 3: Обмеження з'єднань через SemaphoreSlim(3) ---");
//
// SemaphoreSlim — «вертушка» на вході: дозволяє пройти лише N потокам одночасно.
// initialCount: 3 — з самого початку відкрито 3 слоти.
// maxCount: 3    — більше 3 слотів ніколи не буде.
using var semaphore = new SemaphoreSlim(initialCount: 3, maxCount: 3);
//
// Лічильники для підтвердження правильності роботи семафора
int activeConnections = 0;       // скільки з'єднань активні зараз
int maxObservedConnections = 0;  // максимум, що спостерігали одночасно
object maxLock = new();          // lock для безпечного оновлення maxObservedConnections
//
var apiTasks = new Task[10];
for (int i = 0; i < 10; i++)
{
    int apiId = i + 1; // захоплюємо змінну циклу
    // async () => потрібно бо всередині є await
    apiTasks[i] = Task.Run(async () =>
    {
        // WaitAsync — асинхронно чекає на вільний слот.
        // Якщо всі 3 зайняті — задача «засинає» без блокування потоку.
        await semaphore.WaitAsync();
        try
        {
            // Interlocked.Increment — атомарний інкремент (безпечно без lock для int)
            int current = Interlocked.Increment(ref activeConnections);
            // Фіксуємо максимум одночасних з'єднань (потребує lock бо операція не атомарна)
            lock (maxLock)
            {
                if (current > maxObservedConnections)
                    maxObservedConnections = current;
            }
            Console.WriteLine($"  [API {apiId:D2}] Підключено  | активних з'єднань: {current}");
            // Task.Delay — асинхронна пауза (не блокує потік, на відміну від Thread.Sleep)
            await Task.Delay(300); // імітуємо HTTP-запит до API курсу валют
            Console.WriteLine($"  [API {apiId:D2}] Відповідь отримана");
        }
        finally
        {
            // finally виконується ЗАВЖДИ — навіть при винятку.
            // Це гарантує, що слот завжди буде звільнено.
            Interlocked.Decrement(ref activeConnections);
            semaphore.Release(); // +1 слот — наступна задача може увійти
        }
    });
}
await Task.WhenAll(apiTasks);
//
Console.WriteLine();
Console.WriteLine($"Максимум одночасних з'єднань: {maxObservedConnections} (ліміт: 3)");
Console.WriteLine(maxObservedConnections <= 3
    ? "Ліміт дотримано — SemaphoreSlim працює коректно!"
    : "Ліміт перевищено — помилка синхронізації!");
//
// ═════════════════════════════════════════════════════════════════════════════
// Етап 4: Розумна черга (BlockingCollection) — модель «Виробник — Споживач»
// Виробник додає транзакції; Споживач блокується і чекає, поки вони з'являться
// ═════════════════════════════════════════════════════════════════════════════
//
Console.WriteLine();
Console.WriteLine("--- Етап 4: Producer-Consumer через BlockingCollection ---");
//
// BlockingCollection — «розумна» черга-обгортка над ConcurrentQueue.
// boundedCapacity: 20 — якщо в черзі вже 20 елементів і виробник
// намагається додати ще — він ЗАБЛОКУЄТЬСЯ поки споживач не звільнить місце.
// Це захищає від ситуації, коли виробник генерує швидше ніж споживач обробляє.
using var queue = new System.Collections.Concurrent.BlockingCollection<string>(boundedCapacity: 20);
//
int totalProduced = 0; // скільки транзакцій згенеровано
int totalConsumed = 0; // скільки транзакцій оброблено
//
// ── Виробник (Producer) ───────────────────────────────────────────────────────
var producer = Task.Run(() =>
{
    for (int i = 1; i <= 15; i++)
    {
        string transaction = $"[Виробник] Транзакція #{i:D2}: +{i * 50:N0} UAH о {DateTime.Now:HH:mm:ss.fff}";
        // Add — додає елемент у чергу.
        // Якщо черга повна (20 елементів) — потік заблокується тут і чекатиме.
        queue.Add(transaction);
        Interlocked.Increment(ref totalProduced);
        Console.WriteLine($"  => Додано:    {transaction}");
        Thread.Sleep(50); // імітуємо затримку між транзакціями (50 мс)
    }
    // CompleteAdding — ОБОВ'ЯЗКОВИЙ сигнал: більше елементів не буде.
    // Без нього споживач чекав би нових елементів вічно.
    queue.CompleteAdding();
    Console.WriteLine("  [Виробник] Завершив роботу.");
});
//
// ── Споживач (Consumer) ───────────────────────────────────────────────────────
var consumer = Task.Run(() =>
{
    // GetConsumingEnumerable() — ключовий метод:
    // - черга порожня → потік ЗАСИНАЄ (не витрачає CPU у циклі)
    // - з'явився елемент → МИТТЄВО прокидається і обробляє
    // - виробник викликав CompleteAdding() і черга порожня → foreach завершується
    foreach (string transaction in queue.GetConsumingEnumerable())
    {
        Interlocked.Increment(ref totalConsumed);
        Console.WriteLine($"  <= Оброблено: {transaction}");
        Thread.Sleep(80); // споживач трохи повільніший за виробника (80 мс)
    }
    Console.WriteLine("  [Споживач] Завершив роботу.");
});
//
// Чекаємо поки і виробник, і споживач завершать роботу
await Task.WhenAll(producer, consumer);
//
Console.WriteLine();
Console.WriteLine($"Вироблено транзакцій : {totalProduced}");
Console.WriteLine($"Оброблено транзакцій : {totalConsumed}");
Console.WriteLine(totalProduced == totalConsumed
    ? "Всі транзакції оброблені успішно!"
    : "Увага: частина транзакцій не була оброблена!");
//
// ─────────────────────────────────────────────────────────────────────────────
//
/// <summary>
/// Спільний бонусний рахунок біржі для виплати бонусів.
/// </summary>
class BonusAccount
{
    private decimal _balance = 0m;
    //
    // Об'єкт-замок: lock може використовуватись з БУДЬ-ЯКИМ об'єктом,
    // але best practice — окремий приватний об'єкт, щоб ніхто зовні
    // не міг випадково заблокувати той самий замок.
    private readonly object _lock = new();
    //
    // => _balance — скорочений запис властивості (property):
    // еквівалентно: get { return _balance; }
    public decimal Balance => _balance;
    //
    // Небезпечний метод: += це НЕ атомарна операція!
    // Насправді це: READ _balance → ADD amount → WRITE _balance
    // Між цими кроками інший потік може встигнути прочитати старе значення.
    public void AddUnsafe(decimal amount) => _balance += amount;
    //
    // Безпечний метод: lock гарантує що код всередині виконується
    // лише ОДНИМ потоком одночасно. Інші чекають на звільнення замка.
    public void AddSafe(decimal amount)
    {
        lock (_lock) // «закрити двері» для інших потоків
        {
            _balance += amount; // тільки цей потік тут, безпечно
        } // «відкрити двері» — наступний потік може увійти
    }
}