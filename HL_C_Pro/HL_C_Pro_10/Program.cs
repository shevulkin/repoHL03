/*
ДЗ 8. Асинхронний агрегатор крипто-бірж
//
Ви пишете ядро для термінала, який має зібрати ціну біткоїна (BTC) з кількох різних бірж одночасно, щоб знайти найкращу пропозицію. Кожна біржа має свій API, який відповідає з різною швидкістю.
//
Технічні вимоги:
//
1. Імітація запитів до бірж:
Створіть три асинхронних методи: GetBinancePriceAsync, GetCoinbasePriceAsync, GetKrakenPriceAsync.
Кожен метод має генерувати випадкову ціну (наприклад, від 60,000 до 65,000) і використовувати Task.Delay (від 500 мс до 4000 мс) для імітації затримки мережі.
Усі методи повинні приймати CancellationToken.
//
2. Паралельний збір даних (Task.WhenAll):
Програма повинна запустити опитування всіх трьох бірж одночасно.
Після отримання всіх результатів виведіть середню ціну та час, який знадобився на повну перевірку.
//
3. Механізм скасування (CancellationToken):
Поки йде пошук, користувач має бачити напис: "Пошук триває... Натисніть 'S' для скасування".
Якщо користувач натискає 'S', усі асинхронні запити мають негайно зупинитися, а програма — вивести повідомлення: "Операцію скасовано користувачем".
//
4. Режим "Швидка угода" (Task.WhenAny):
Додайте логіку: якщо нам потрібно отримати хоча б одну ціну якомога швидше, ми використовуємо Task.WhenAny.
Програма має вивести першу ціну, яка прийшла, і назву біржі, яка "перемогла" у швидкості.
//
Обробка таймауту:
Якщо жодна біржа не відповіла протягом 2.5 секунд, програма має видати помилку: "Мережевий таймаут: біржі не відповідають".
*/
using System.Diagnostics;

class Program
{
    private static readonly Random random = new Random();
    // --- Точка входу: послідовно запускаємо обидва режими ---
    static async Task Main(string[] args)
    {
        // Режим 2+3: повний збір цін з можливістю скасування
        await RunWhenAllMode();
        Console.WriteLine();
        //
        // Режим 4: швидка угода — перша відповідь виграє
        await RunWhenAnyMode();
    }
    // --- 1. Імітація запитів до бірж ---
    //
    // Імітація запиту до Binance: випадкова ціна BTC + випадкова затримка мережі
    static async Task<decimal> GetBinancePriceAsync(CancellationToken ct)
    {
        int delay = random.Next(500, 4001); // затримка від 500 до 4000 мс
        await Task.Delay(delay, ct);        // чекаємо з підтримкою скасування
        return Math.Round((decimal)(random.NextDouble() * 5000 + 60000), 2); // ціна від 60 000 до 65 000
    }
    //
    // Імітація запиту до Coinbase: випадкова ціна BTC + випадкова затримка мережі
    static async Task<decimal> GetCoinbasePriceAsync(CancellationToken ct)
    {
        int delay = random.Next(500, 4001); // затримка від 500 до 4000 мс
        await Task.Delay(delay, ct);        // чекаємо з підтримкою скасування
        return Math.Round((decimal)(random.NextDouble() * 5000 + 60000), 2); // ціна від 60 000 до 65 000
    }
    //
    // Імітація запиту до Kraken: випадкова ціна BTC + випадкова затримка мережі
    static async Task<decimal> GetKrakenPriceAsync(CancellationToken ct)
    {
        int delay = random.Next(500, 4001); // затримка від 500 до 4000 мс
        await Task.Delay(delay, ct);        // чекаємо з підтримкою скасування
        return Math.Round((decimal)(random.NextDouble() * 5000 + 60000), 2); // ціна від 60 000 до 65 000
    }
    // --- 2. Паралельний збір даних (Task.WhenAll) ---
    static async Task RunWhenAllMode()
    {
        using var cts = new CancellationTokenSource();
        //
        Console.WriteLine("\n=== Режим: Повний збір цін (Task.WhenAll) ===");
        Console.WriteLine("Пошук триває... Натисніть 'S' для скасування\n"); // 3. Механізм скасування (CancellationToken)
        //
        var stopwatch = Stopwatch.StartNew(); // запускаємо таймер для вимірювання загального часу
        //
        // Запускаємо три запити одночасно (паралельно)
        var binanceTask = GetBinancePriceAsync(cts.Token);
        var coinbaseTask = GetCoinbasePriceAsync(cts.Token);
        var krakenTask = GetKrakenPriceAsync(cts.Token);
        var allPricesTask = Task.WhenAll(binanceTask, coinbaseTask, krakenTask); // чекаємо всіх трьох
        //
        // Слухаємо натискання клавіші 'S' у фоновому потоці для скасування операції
        var cancelTask = Task.Run(() =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.S)
                    {
                        Console.WriteLine("\nЗапит на скасування отримано, зупиняємо біржі...");
                        cts.Cancel(); // сигналізуємо про скасування всім завданням
                        break;
                    }
                }
                Thread.Sleep(50);
            }
        });
        //
        try
        {
            decimal[] prices = await allPricesTask; // отримуємо ціни від усіх бірж
            stopwatch.Stop();
            //
            Console.WriteLine($"Binance:  {prices[0]:F2} USD");
            Console.WriteLine($"Coinbase: {prices[1]:F2} USD");
            Console.WriteLine($"Kraken:   {prices[2]:F2} USD");
            Console.WriteLine($"\nСередня ціна: {prices.Average():F2} USD");
            Console.WriteLine($"Час перевірки: {stopwatch.Elapsed.TotalSeconds:F2} с");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nОперацію скасовано користувачем.");
        }
        finally
        {
            cts.Cancel(); // зупиняємо фоновий слухач клавіші, якщо він ще працює
        }
    }
    // --- 4. Режим "Швидка угода" (Task.WhenAny) ---
    static async Task RunWhenAnyMode()
    {
        using var cts = new CancellationTokenSource();
        //
        Console.WriteLine("\n=== Режим: Швидка угода (Task.WhenAny) ===");
        //всі три стартують одночасно — прямо під час ініціалізації масиву
        var exchanges = new (string Name, Task<decimal> Task)[]
        {
            ("Binance",  GetBinancePriceAsync(cts.Token)),
            ("Coinbase", GetCoinbasePriceAsync(cts.Token)),
            ("Kraken",   GetKrakenPriceAsync(cts.Token)),
        };
        //timeoutTask стартує одночасно з біржами
        var timeoutTask = Task.Delay(2500, cts.Token); // Обробка таймауту: якщо жодна біржа не відповіла за 2.5 с
        var priceTasks = exchanges.Select(e => e.Task).ToArray();
        //
        // Чекаємо першого завершеного завдання: або біржа відповіла, або спрацював таймаут
        var winner = await Task.WhenAny(priceTasks.Concat([timeoutTask]).ToArray());
        if (winner == timeoutTask)
        {
            Console.WriteLine("Мережевий таймаут: біржі не відповідають.");
            cts.Cancel();
            return;
        }
        //
        // Визначаємо, яка біржа відповіла першою
        for (int i = 0; i < exchanges.Length; i++)
        {
            if (winner == exchanges[i].Task)
            {
                decimal price = await exchanges[i].Task;
                Console.WriteLine($"Перша відповідь від: {exchanges[i].Name}");
                Console.WriteLine($"Ціна BTC: {price:F2} USD");
                break;
            }
        }
        // скасовуємо запити до решти бірж, що ще не відповіли
        cts.Cancel();
    }
}