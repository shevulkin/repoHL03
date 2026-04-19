using System.Windows;
using System.Windows.Controls;

namespace MyApp
{
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            InitializeComponent();
            DataContext = new ShellViewModel();
        }

        // PasswordBox не підтримує Binding — оновлюємо ViewModel напряму
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb && pb.DataContext is LoginViewModel vm)
                vm.Password = pb.Password;
        }
    }
}
