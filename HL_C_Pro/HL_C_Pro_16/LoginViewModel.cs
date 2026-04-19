using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyApp
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _login = "";
        private string _password = "";
        private string _statusMessage = "";

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanLogin));
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // Кнопка активна тільки коли обидва поля заповнені
        public bool CanLogin =>
            !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);

        public ICommand LoginCommand { get; }

        // Event — викликається після успішного входу
        public event Action? OnLoginSuccess;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(
                execute:    _ => OnLoginSuccess?.Invoke(),
                canExecute: _ => CanLogin
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
