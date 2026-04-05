using Microsoft.AspNetCore.Mvc;
using MovieManager.Models;
using MovieManager.Services;

public class MoviesController : Controller
{
    private readonly MovieService _service;
    private readonly IWebHostEnvironment _environment;

    public MoviesController(MovieService service, IWebHostEnvironment environment)
    {
        _service = service;
        _environment = environment;
    }

    // Вивід списку з фільтрацією
    public IActionResult Index(string title, string[] genres, int[] years, string sortBy = "id_desc")
    {
        var allMovies = _service.GetAll();
        var movies = allMovies.AsQueryable();

        // Основний пошук по назві (завжди активний)
        if (!string.IsNullOrEmpty(title))
        {
            var titleFilter = title.Trim().ToLower();
            movies = movies.Where(m => m.Title.ToLower().Contains(titleFilter));
        }

        // Фільтр по жанрам (якщо вибрані)
        if (genres != null && genres.Length > 0)
        {
            var selectedGenres = genres.Where(g => !string.IsNullOrWhiteSpace(g)).Select(g => g.ToLower()).ToList();
            if (selectedGenres.Count > 0)
                movies = movies.Where(m => selectedGenres.Contains(m.Genre.ToLower()));
        }

        // Фільтр по рокам (якщо вибрані)
        if (years != null && years.Length > 0)
            movies = movies.Where(m => years.Contains(m.Year));

        // Сортування
        movies = sortBy switch
        {
            "title_asc"  => movies.OrderBy(m => m.Title),
            "title_desc" => movies.OrderByDescending(m => m.Title),
            "year_asc"   => movies.OrderBy(m => m.Year),
            "year_desc"  => movies.OrderByDescending(m => m.Year),
            "genre_asc"  => movies.OrderBy(m => m.Genre),
            "genre_desc" => movies.OrderByDescending(m => m.Genre),
            "id_asc"     => movies.OrderBy(m => m.Id),
            _            => movies.OrderByDescending(m => m.Id),
        };

        // Передаємо до View список доступних жанрів і років
        ViewBag.AllGenres = allMovies.Select(m => m.Genre).Distinct().OrderBy(g => g).ToList();
        ViewBag.AllYears = allMovies.Select(m => m.Year).Distinct().OrderByDescending(y => y).ToList();
        ViewBag.SelectedGenres = genres?.Where(g => !string.IsNullOrWhiteSpace(g)).ToList() ?? new List<string>();
        ViewBag.SelectedYears = years?.Where(y => y > 0).ToList() ?? new List<int>();
        ViewBag.SortBy = sortBy;
        ViewBag.CurrentTitle = title;

        return View(movies.ToList());
    }

    // Перегляд фільму
    public IActionResult Details(int id)
    {
        var movie = _service.GetById(id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    // Створення (GET та POST)
    public IActionResult Create() => View(new Movie());

    [HttpPost]
    public IActionResult Create(Movie movie, IFormFile? movieFile = null)
    {
        if (movie == null)
            return View(new Movie());

        // Видаляємо автоматичну валідацію для опціональних полів
        ModelState.Remove("MovieFilePath");
        ModelState.Remove("MovieUrl");

        // Нормалізуємо порожній рядок у null
        if (string.IsNullOrWhiteSpace(movie.MovieUrl))
            movie.MovieUrl = null;
        else if (!movie.MovieUrl.StartsWith("http://") && !movie.MovieUrl.StartsWith("https://"))
            movie.MovieUrl = "https://" + movie.MovieUrl.Trim();

        if (ModelState.IsValid)
        {
            // Обробка завантаження файла
            if (movieFile != null && movieFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(movieFile.FileName);
                var uploadPath = Path.Combine(_environment.WebRootPath, "movies");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    movieFile.CopyTo(stream);
                }

                movie.MovieFilePath = $"/movies/{fileName}";
                movie.MovieUrl = null; // Очищуємо URL якщо вибрано файл
            }
            else if (!string.IsNullOrEmpty(movie.MovieUrl))
            {
                // Якщо вибрано URL замість файла
                movie.MovieFilePath = null;
                // Зберігаємо посилання як є
            }

            _service.Add(movie);
            return RedirectToAction(nameof(Index));
        }
        return View(movie);
    }

    // Редагування (GET та POST)
    public IActionResult Edit(int id, string? returnUrl = null)
    {
        var movie = _service.GetById(id);
        if (movie == null) return NotFound();
        ViewBag.ReturnUrl = returnUrl;
        return View(movie);
    }

    [HttpPost]
    public IActionResult Edit(Movie movie, IFormFile? movieFile = null, bool removeMedia = false, string? returnUrl = null)
    {
        if (movie == null)
            return RedirectToAction(nameof(Index));

        // Видаліть автоматичну валідацію для необов'язкових полів
        ModelState.Remove("MovieFilePath");
        ModelState.Remove("MovieUrl");

        // Нормалізуємо порожній рядок у null
        if (string.IsNullOrWhiteSpace(movie.MovieUrl))
            movie.MovieUrl = null;
        else if (!movie.MovieUrl.StartsWith("http://") && !movie.MovieUrl.StartsWith("https://"))
            movie.MovieUrl = "https://" + movie.MovieUrl.Trim();

        if (ModelState.IsValid)
        {
            var existingMovie = _service.GetById(movie.Id);
            if (existingMovie == null) return NotFound();

            // Якщо завантажено новий файл
            if (movieFile != null && movieFile.Length > 0)
            {
                // Видаляємо старий файл
                if (!string.IsNullOrEmpty(existingMovie.MovieFilePath))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, existingMovie.MovieFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                // Завантажуємо новий файл
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(movieFile.FileName);
                var uploadPath = Path.Combine(_environment.WebRootPath, "movies");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    movieFile.CopyTo(stream);
                }

                movie.MovieFilePath = $"/movies/{fileName}";
                movie.MovieUrl = null; // Очищуємо URL якщо вибрано файл
            }
            else if (!string.IsNullOrEmpty(movie.MovieUrl))
            {
                // Якщо вибрано URL, видаляємо старий файл
                if (!string.IsNullOrEmpty(existingMovie.MovieFilePath))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, existingMovie.MovieFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }
                movie.MovieFilePath = null;
                // Зберігаємо посилання як є
            }
            else if (removeMedia)
            {
                // Вилучаємо поточне медіа
                if (!string.IsNullOrEmpty(existingMovie.MovieFilePath))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, existingMovie.MovieFilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }
                movie.MovieFilePath = null;
                movie.MovieUrl = null;
            }
            else
            {
                // Якщо нічого не вибрано, зберігаємо що було
                movie.MovieFilePath = existingMovie.MovieFilePath;
                movie.MovieUrl = existingMovie.MovieUrl;
            }

            _service.Update(movie);
            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction(nameof(Index));
        }
        ViewBag.ReturnUrl = returnUrl;
        return View(movie);
    }

    // Видалення — підтвердження (GET)
    public IActionResult Delete(int id)
    {
        var movie = _service.GetById(id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    // Видалення — виконання (POST)
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var movie = _service.GetById(id);
        if (movie != null && !string.IsNullOrEmpty(movie.MovieFilePath))
        {
            var filePath = Path.Combine(_environment.WebRootPath, movie.MovieFilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        _service.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}