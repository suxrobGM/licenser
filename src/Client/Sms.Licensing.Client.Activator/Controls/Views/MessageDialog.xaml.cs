using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;

namespace Sms.Licensing.Client.Activator.Controls
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public MessageDialog()
        {
            InitializeComponent();
            OkButtonCommand = new ActionCommand(() => ShowDialog = false);
            CancelButtonCommand = new ActionCommand(() => ShowDialog = false);
        }

        public static readonly DependencyProperty ShowDialogProperty = DependencyProperty.Register(
            "ShowDialog", typeof(bool), typeof(MessageDialog), new PropertyMetadata(default(bool)));

        public bool ShowDialog
        {
            get => (bool) GetValue(ShowDialogProperty);
            set => SetValue(ShowDialogProperty, value);
        }

        #region Message and caption dependency properties

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            "Caption", typeof(string), typeof(MessageDialog), new PropertyMetadata(default(string)));

        public string Caption
        {
            get => (string) GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }


        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register(
            "MessageText", typeof(string), typeof(MessageDialog), new PropertyMetadata(default(string)));

        public string MessageText
        {
            get => (string) GetValue(MessageTextProperty);
            set => SetValue(MessageTextProperty, value);
        }


        public static readonly DependencyProperty CaptionFontSizeProperty = DependencyProperty.Register(
            "CaptionFontSize", typeof(double), typeof(MessageDialog), new PropertyMetadata(14.0));

        public double CaptionFontSize
        {
            get => (double) GetValue(CaptionFontSizeProperty);
            set => SetValue(CaptionFontSizeProperty, value);
        }


        public static readonly DependencyProperty MessageTextFontSizeProperty = DependencyProperty.Register(
            "MessageTextFontSize", typeof(double), typeof(MessageDialog), new PropertyMetadata(14.0));

        public double MessageTextFontSize
        {
            get => (double) GetValue(MessageTextFontSizeProperty);
            set => SetValue(MessageTextFontSizeProperty, value);
        }

        #endregion

        #region Buttons dependency properties

        #region OK button

        public static readonly DependencyProperty OkButtonCommandProperty = DependencyProperty.Register(
            "OkButtonCommand", typeof(ICommand), typeof(MessageDialog), new PropertyMetadata(default(ICommand)));

        public ICommand OkButtonCommand
        {
            get => (ICommand) GetValue(OkButtonCommandProperty);
            set => SetValue(OkButtonCommandProperty, value);
        }



        public static readonly DependencyProperty OkButtonVisibleProperty = DependencyProperty.Register(
            "OkButtonVisible", typeof(bool), typeof(MessageDialog), new PropertyMetadata(default(bool)));

        public bool OkButtonVisible
        {
            get => (bool) GetValue(OkButtonVisibleProperty);
            set => SetValue(OkButtonVisibleProperty, value);
        }



        public static readonly DependencyProperty OkButtonFontSizeProperty = DependencyProperty.Register(
            "OkButtonFontSize", typeof(double), typeof(MessageDialog), new PropertyMetadata(14.0));

        public double OkButtonFontSize
        {
            get => (double) GetValue(OkButtonFontSizeProperty);
            set => SetValue(OkButtonFontSizeProperty, value);
        }

        public static readonly DependencyProperty OkButtonHeightProperty = DependencyProperty.Register(
            "OkButtonHeight", typeof(double), typeof(MessageDialog), new PropertyMetadata(30.0));

        public double OkButtonHeight
        {
            get => (double) GetValue(OkButtonHeightProperty);
            set => SetValue(OkButtonHeightProperty, value);
        }

        public static readonly DependencyProperty OkButtonWidthProperty = DependencyProperty.Register(
            "OkButtonWidth", typeof(double), typeof(MessageDialog), new PropertyMetadata(50.0));

        public double OkButtonWidth
        {
            get => (double) GetValue(OkButtonWidthProperty);
            set => SetValue(OkButtonWidthProperty, value);
        }

        #endregion

        #region Cancel button

        public static readonly DependencyProperty CancelButtonCommandProperty = DependencyProperty.Register(
            "CancelButtonCommand", typeof(ICommand), typeof(MessageDialog), new PropertyMetadata(default(ICommand)));

        public ICommand CancelButtonCommand
        {
            get => (ICommand) GetValue(CancelButtonCommandProperty);
            set => SetValue(CancelButtonCommandProperty, value);
        }



        public static readonly DependencyProperty CancelButtonVisibleProperty = DependencyProperty.Register(
            "CancelButtonVisible", typeof(bool), typeof(MessageDialog), new PropertyMetadata(default(bool)));

        public bool CancelButtonVisible
        {
            get => (bool) GetValue(CancelButtonVisibleProperty);
            set => SetValue(CancelButtonVisibleProperty, value);
        }



        public static readonly DependencyProperty CancelButtonFontSizeProperty = DependencyProperty.Register(
            "CancelButtonFontSize", typeof(double), typeof(MessageDialog), new PropertyMetadata(14.0));

        public double CancelButtonFontSize
        {
            get => (double) GetValue(CancelButtonFontSizeProperty);
            set => SetValue(CancelButtonFontSizeProperty, value);
        }



        public static readonly DependencyProperty CancelButtonHeightProperty = DependencyProperty.Register(
            "CancelButtonHeight", typeof(double), typeof(MessageDialog), new PropertyMetadata(30.0));

        public double CancelButtonHeight
        {
            get => (double) GetValue(CancelButtonHeightProperty);
            set => SetValue(CancelButtonHeightProperty, value);
        }



        public static readonly DependencyProperty CancelButtonWidthProperty = DependencyProperty.Register(
            "CancelButtonWidth", typeof(double), typeof(MessageDialog), new PropertyMetadata(50.0));

        public double CancelButtonWidth
        {
            get => (double) GetValue(CancelButtonWidthProperty);
            set => SetValue(CancelButtonWidthProperty, value);
        }

        #endregion

        #endregion
    }
}
