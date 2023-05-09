using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using WinRT.Interop;

namespace WinUI3Demo
{
    public sealed partial class LoginWindow : Window
    {
        #region Properties

        public LoginViewModel ViewModel { get; } = new();

        #endregion

        #region Constructor

        public LoginWindow()
        {
            this.InitializeComponent();
            ResizeAndCenterWindow();
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        #endregion

        #region Events

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ValidateLogin()) 
                DisplaySuccessDialog();
        }
        private void OnEmailTextChanged(object sender, RoutedEventArgs e)
        {
            SetDefaultTextBoxFormatting((TextBox)sender);
            ViewModel.EmailErrorText = string.Empty;
        }

        private void OnPasswordTextChanged(object sender, RoutedEventArgs e)
        {
            SetDefaultPasswordBoxFormatting((PasswordBox)sender);
            ViewModel.PasswordErrorText = string.Empty;
        }

        private void ShowPasswordButton_PointerPressed(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.PasswordRevealMode = PasswordRevealMode.Visible;
            ShowPasswordButton.ClickMode = ClickMode.Release;
            ShowPasswordButton.Click -= ShowPasswordButton_PointerPressed;
            ShowPasswordButton.Click += ShowPasswordButton_PointerReleased;
        }

        private void ShowPasswordButton_PointerReleased(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.PasswordRevealMode = PasswordRevealMode.Hidden;
            ShowPasswordButton.ClickMode = ClickMode.Press;
            ShowPasswordButton.Click -= ShowPasswordButton_PointerReleased;
            ShowPasswordButton.Click += ShowPasswordButton_PointerPressed;
        }

        private void TextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                OnLoginButtonClick(sender, e);
            }
        }
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not LoginViewModel viewModel) return;

            if (string.IsNullOrEmpty(viewModel.EmailErrorText))
                SetDefaultTextBoxFormatting(EmailTextBox);
            else
                SetErrorTextBoxFormatting(EmailTextBox);

            if (string.IsNullOrEmpty(viewModel.PasswordErrorText))
                SetDefaultPasswordBoxFormatting(PasswordTextBox);
            else
                SetErrorPasswordBoxFormatting(PasswordTextBox);
        }

        #endregion

        #region Methods

        private void ResizeAndCenterWindow()
        {
            var windowHandle = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var presenter = appWindow.Presenter as OverlappedPresenter;
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest);

            appWindow.Resize(new SizeInt32 { Width = 500, Height = 570 });
            presenter.IsResizable = false;
            presenter.SetBorderAndTitleBar(false, false);

            var centeredPosition = new PointInt32()
            {
                X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2,
                Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2
            };
            appWindow.Move(centeredPosition);

        }

        private async void DisplaySuccessDialog()
        {
            var dialog = new ContentDialog
            {
                Title = "Success",
                Content = "Account created",
                CloseButtonText = "OK",
                XamlRoot = MainPanel.XamlRoot
            };

            await dialog.ShowAsync();
            Application.Current.Exit();
        }

        private void SetDefaultTextBoxFormatting(TextBox textBox)
        {
            textBox.Style = (Style)Application.Current.Resources["TextBoxStyle"];
        }
        private void SetDefaultPasswordBoxFormatting(PasswordBox passwordBox)
        {
            passwordBox.Style = (Style)Application.Current.Resources["PasswordBoxStyle"];
        }

        private void SetErrorTextBoxFormatting(TextBox textBox)
        {
            textBox.Style = (Style)Application.Current.Resources["TextBoxErrorStyle"];
        }
        private void SetErrorPasswordBoxFormatting(PasswordBox passwordBox)
        {
            var ErrorColor = (Windows.UI.Color)Application.Current.Resources["ErrorColor"];

            passwordBox.Style = (Style)Application.Current.Resources["PasswordBoxErrorStyle"];
            var placeholderColorBrush = passwordBox.Resources["TextControlPlaceholderForeground"] as SolidColorBrush;
            placeholderColorBrush.Color = ErrorColor;
            placeholderColorBrush = passwordBox.Resources["TextControlPlaceholderForegroundFocused"] as SolidColorBrush;
            placeholderColorBrush.Color = ErrorColor;
            placeholderColorBrush = passwordBox.Resources["TextControlPlaceholderForegroundPointerOver"] as SolidColorBrush;
            placeholderColorBrush.Color = ErrorColor;
        }

        #endregion
    }
}
