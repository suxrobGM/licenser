using System.Windows;
using System.Windows.Controls;

namespace Sms.Licensing.Client.Activator.Controls
{
    /// <summary>
    /// Interaction logic for BusyIndicator.xaml
    /// </summary>
    public partial class BusyIndicator : UserControl
    {
        public BusyIndicator()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LoadingTextProperty = DependencyProperty.Register(
            "LoadingText", typeof(string), typeof(BusyIndicator), new PropertyMetadata(default(string)));

        public string LoadingText
        {
            get => (string) GetValue(LoadingTextProperty);
            set => SetValue(LoadingTextProperty, value);
        }

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            "IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(default(bool)));

        public bool IsBusy
        {
            get => (bool) GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public static readonly DependencyProperty LoadingTextFontSizeProperty = DependencyProperty.Register(
            "LoadingTextFontSize", typeof(int), typeof(BusyIndicator), new PropertyMetadata(default(int)));

        public int LoadingTextFontSize
        {
            get => (int) GetValue(LoadingTextFontSizeProperty);
            set => SetValue(LoadingTextFontSizeProperty, value);
        }
    }
}
