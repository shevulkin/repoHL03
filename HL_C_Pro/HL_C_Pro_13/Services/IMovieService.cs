using MovieManager.Models;

namespace MovieManager.Services
{
    public class MovieService
    {
        // Статичний список для зберігання даних під час сеансу
        private static readonly List<Movie> _movies = new()
        {
            new Movie { Id = 1, Title = "Інтерстеллар", Genre = "Sci-Fi", Year = 2014 },
            new Movie { Id = 2, Title = "Хрещений батько", Genre = "Drama", Year = 1972 },
            new Movie { Id = 3, Title = "Матриця", Genre = "Sci-Fi", Year = 1999 },
            new Movie { Id = 4, Title = "Форест Гамп", Genre = "Drama", Year = 1994 },
            new Movie { Id = 5, Title = "Темний лицар", Genre = "Action", Year = 2008 },
            new Movie { Id = 6, Title = "Король лев", Genre = "Animation", Year = 1994 },
            new Movie { Id = 7, Title = "16. Asp Net Core MVC", Genre = "Study", Year = 2024,
                MovieUrl="https://www.youtube.com/watch?v=frguien8lW8" }
        };

        public List<Movie> GetAll() => _movies;

        public void Add(Movie movie)
        {
            movie.Id = _movies.Any() ? _movies.Max(m => m.Id) + 1 : 1;
            _movies.Add(movie);
        }

        public Movie? GetById(int id) => _movies.FirstOrDefault(m => m.Id == id);

        public void Update(Movie movie)
        {
            var index = _movies.FindIndex(m => m.Id == movie.Id);
            if (index != -1) _movies[index] = movie;
        }

        public void Delete(int id) => _movies.RemoveAll(m => m.Id == id);
    }
}