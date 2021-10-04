using System.Windows;
using System.Windows.Controls;
using Sms.Licensing.Client.Activator.ViewModels;

namespace Sms.Licensing.Client.Activator.Views
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                ((LoginPageViewModel)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
