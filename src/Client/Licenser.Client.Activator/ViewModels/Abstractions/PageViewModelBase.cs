using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
// ReSharper disable MemberCanBeProtected.Global

namespace Licenser.Client.Activator.ViewModels.Abstractions
{
    public abstract class PageViewModelBase : BindableBase
    {
        protected readonly IRegionManager _regionManager;

        protected PageViewModelBase(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            PageLoadedCommand = new DelegateCommand(async () => await LoadPageAsync());
        }

        /// <summary>
        /// Event invokes when value of IsBusy property was changed.
        /// </summary>
        protected event EventHandler OnBusyAction;

        #region Commands

        public DelegateCommand PageLoadedCommand { get; }

        #endregion

        #region Bindable Properties

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);

                if (value == false)
                    LoadingText = "";

                OnBusyAction?.Invoke(this, EventArgs.Empty);
            } 
        }

        private string _loadingText;
        public string LoadingText
        {
            get => _loadingText;
            set => SetProperty(ref _loadingText, value);
        }

        #endregion

        protected abstract Task LoadPageAsync();
    }
}