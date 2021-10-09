using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Prism.Regions;

namespace Licenser.Client.Activator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();
            regionManager.RegisterViewWithRegion("MainPageFrame", typeof(LoginPage)); // Startup page
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Content == null || (e.Content as string) == string.Empty)
                return;

            var thicknessAnimation = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.3),
                DecelerationRatio = 0.7,
                To = new Thickness(0, 0, 0, 0)
            };

            thicknessAnimation.From = e.NavigationMode switch
            {
                NavigationMode.New => new Thickness(500, 0, 0, 0),
                NavigationMode.Back => new Thickness(0, 0, 500, 0),
                _ => thicknessAnimation.From
            };

            (e.Content as Page)?.BeginAnimation(MarginProperty, thicknessAnimation);
        }
    }
}
