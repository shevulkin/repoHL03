using System.Windows;
using System.Windows.Controls;

namespace MyApp
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        private static bool _updating = false;

        public static string GetBoundPassword(DependencyObject obj) =>
            (string)obj.GetValue(BoundPasswordProperty);

        public static void SetBoundPassword(DependencyObject obj, string value) =>
            obj.SetValue(BoundPasswordProperty, value);

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not PasswordBox box) return;
            box.PasswordChanged -= PasswordChanged;

            if (!_updating)
                box.Password = (string)e.NewValue;

            box.PasswordChanged += PasswordChanged;
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not PasswordBox box) return;
            _updating = true;
            SetBoundPassword(box, box.Password);
            _updating = false;
        }
    }
}
