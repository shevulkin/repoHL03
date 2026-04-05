/*
 * ДЗ 11. Музичний плеєр

Зробити програму, яка імітує роботу музичного плеєра
Задати довільний плейліст, який буде обирати випадкову пісню зі списку, але за умови, що ця пісня не була серед 5 останніх програних
Приділити особливу увагу оптимальності реалізації
*/
//
class Program
{
    static void Main()
    {
        Console.Clear();
        string[] songs = new string[]
        {
            "Song 1", "Song 2", "Song 3", "Song 4", "Song 5",
            "Song 6", "Song 7", "Song 8", "Song 9", "Song 10"
        };
        int iterations = 100_000;
        Console.WriteLine($"Виконуємо {iterations} ітерацій випадкової пісні для кожного рішення");
        //Рішення з (do/while + Queue)
        MusicPlayerQueue queuePlayer = new MusicPlayerQueue(songs);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            queuePlayer.PlayRandomSong();
        sw.Stop();
        long queueMs = sw.ElapsedMilliseconds;
        Console.WriteLine($"Старий (Queue + do/while):  {queueMs} ms");
        //
        //Рішення з (HashSet + фільтрація)
        MusicPlayerHasSet hashSetPlayer = new MusicPlayerHasSet(songs);
        sw.Restart();
        for (int i = 0; i < iterations; i++)
            hashSetPlayer.PlayRandomSong();
        sw.Stop();
        long hashSetMs = sw.ElapsedMilliseconds;
        Console.WriteLine($"Новий (HashSet + Where):    {hashSetMs} ms");
        //
        string winner;
        if (queueMs < hashSetMs)
        {
            winner = "Queue + do/while";
            Console.WriteLine($"Переможець: {winner}. Швидше у {(double)hashSetMs / queueMs:F1} разів");
        }
        else
        {
            winner = "HashSet + Where";
            Console.WriteLine($"Переможець: {winner}. Швидше у {(double)queueMs / hashSetMs:F1} разів");
        }
    }
}
//
// Рішення з (Queue + do/while)
public class MusicPlayerQueue
{
    public MusicPlayerQueue(string[] playlist)
    {
        Playlist = playlist;
        RecentSongs = new Queue<string>();
    }
    public string[] Playlist { get; set; }
    private Queue<string> RecentSongs { get; set; }
    public string PlayRandomSong()
    {
        string song;
        do
        {
            int index = Random.Shared.Next(0, Playlist.Length);
            song = Playlist[index];
        } while (RecentSongs.Contains(song));
        if (RecentSongs.Count >= 5)
            RecentSongs.Dequeue();
        RecentSongs.Enqueue(song);
        return song;
    }
}
// Рішення з (HashSet + фільтрація)
public class MusicPlayerHasSet
{
    public MusicPlayerHasSet(string[] playlist)
    {
        Playlist = playlist;
        RecentSongs = new Queue<string>(6);
        RecentSet = new HashSet<string>(6);
    }
    public string[] Playlist { get; set; }
    private Queue<string> RecentSongs { get; set; }
    private HashSet<string> RecentSet { get; set; }
    //
    public string PlayRandomSong()
    {
        // Будуємо список доступних пісень — тих яких немає в останніх 5
        string[] available = Playlist.Where(s => !RecentSet.Contains(s)).ToArray();
        int index = Random.Shared.Next(0, available.Length);
        string song = available[index];
        // Додаємо в чергу останніх
        if (RecentSongs.Count >= 5)
        {
            string oldest = RecentSongs.Dequeue();
            RecentSet.Remove(oldest); // видаляємо найстарішу
        }
        RecentSongs.Enqueue(song);
        RecentSet.Add(song);
        return song;
    }
}