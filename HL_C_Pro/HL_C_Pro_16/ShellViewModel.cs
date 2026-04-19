using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyApp
{
    public class ShellViewModel : INotifyPropertyChanged
    {
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ShellViewModel()
        {
            var loginVm = new LoginViewModel();

            // Коли логін успішний — переключаємо на головний екран
            loginVm.OnLoginSuccess += () => CurrentView = new PersonModel();

            _currentView = loginVm;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
