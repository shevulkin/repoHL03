using System.Windows;
using System.Windows.Controls;

namespace MyApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var vm = new LoginViewModel();

            // Коли вхід успішний — відкрити головне вікно і закрити це
            vm.OnLoginSuccess += () =>
            {
                new MainWindow().Show();
                Close();
            };

            DataContext = vm;
        }

        // PasswordBox не підтримує Binding напряму — оновлюємо ViewModel вручну
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
