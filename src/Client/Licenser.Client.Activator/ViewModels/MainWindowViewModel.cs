using Licenser.Client.Activator.Settings;
using Microsoft.Extensions.Configuration;
using Prism.Mvvm;

namespace Licenser.Client.Activator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(IConfiguration configuration)
        {
            var windowSettings = configuration.GetSection("WindowSettings").Get<WindowSettings>();

            if (windowSettings != null)
            {
                Width = windowSettings.Width;
                Height = windowSettings.Height;
            }
            else
            {
                Width = 600;
                Height = 800;
            }
        }

        #region Bindable properties

        private int _width;
        public int Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }


        private int _height;

        public int Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }
        #endregion
    }
}