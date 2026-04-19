using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyApp
{
    public class PersonModel : INotifyPropertyChanged
    {
        private string _name = "";
        private string _email = "";

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); OnPropertyChanged(nameof(Greeting)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        // Обчислювана властивість — оновлюється автоматично
        public string Greeting => string.IsNullOrWhiteSpace(Name)
            ? "Введи ім'я..."
            : $"Привіт, {Name}!";

        private int _clickCount = 0;
        public int ClickCount
        {
            get => _clickCount;
            set { _clickCount = value; OnPropertyChanged(); }
        }

        // Команди
        public ICommand IncrementCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ClearFormCommand { get; }

        public PersonModel()
        {
            // Команда для збільшення лічильника
            IncrementCommand = new RelayCommand(
                execute: _ => ClickCount++,
                canExecute: _ => true
            );

            // Команда для скидання лічильника
            ResetCommand = new RelayCommand(
                execute: _ => ClickCount = 0,
                canExecute: _ => ClickCount > 0
            );

            // Команда для очищення форми
            ClearFormCommand = new RelayCommand(
                execute: _ =>
                {
                    Name = "";
                    Email = "";
                    ClickCount = 0;
                },
                canExecute: _ => !string.IsNullOrWhiteSpace(Name) || !string.IsNullOrWhiteSpace(Email) || ClickCount > 0
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}