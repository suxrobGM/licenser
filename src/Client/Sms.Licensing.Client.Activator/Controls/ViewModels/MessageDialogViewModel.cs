using System;
using Prism.Services.Dialogs;

namespace Sms.Licensing.Client.Activator.Controls
{
    public class MessageDialogViewModel : IDialogAware
    {
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }

        public string Title { get; } = "Message Dialog";
        public event Action<IDialogResult> RequestClose;
    }
}