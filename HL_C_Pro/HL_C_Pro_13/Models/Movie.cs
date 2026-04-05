using System.ComponentModel.DataAnnotations;

namespace MovieManager.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        [Display(Name = "Назва фільму")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Жанр обов'язковий")]
        public string Genre { get; set; } = string.Empty;

        [Range(1888, 2100, ErrorMessage = "Рік має бути від 1888 до 2100")]
        [Display(Name = "Рік випуску")]
        public int Year { get; set; } = 2024;

        [Display(Name = "Файл фільму")]
        public string? MovieFilePath { get; set; }

        [Display(Name = "Посилання на фільм (URL)")]
        public string? MovieUrl { get; set; }
    }
}