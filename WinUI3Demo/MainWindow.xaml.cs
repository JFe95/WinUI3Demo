using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml.Media;
using Windows.Graphics;
using WinRT.Interop;

namespace WinUI3Demo
{
    public sealed partial class MainWindow : Window
    {

        #region Constructor

        public MainWindow()
        {
            this.InitializeComponent();
            ResizeAndCenterWindow();            
        }

        #endregion

        #region Events

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (ValidateLogin()) 
                DisplaySuccessDialog();
        }
        private void OnEmailTextChanged(object sender, RoutedEventArgs e)
        {
            SetDefaultTextBoxFormatting((TextBox)sender);
            UsernameErrorTextBlock.Text = string.Empty;
        }

        private void OnPasswordTextChanged(object sender, RoutedEventArgs e)
        {
            SetDefaultPasswordBoxFormatting((PasswordBox)sender);
            PasswordErrorTextBlock.Text = string.Empty;
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

        #endregion

        #region Methods

        public void ResizeAndCenterWindow()
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

        private bool ValidateLogin() { return ValidateEmail() & ValidatePassword(); }

        private bool ValidateEmail()
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameErrorTextBlock.Text = "Email is a required field";
                SetErrorTextBoxFormatting(UsernameTextBox);
                return false;
            }
            return true;
        }

        private bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                PasswordErrorTextBlock.Text = "Password is a required field";
                SetErrorPasswordBoxFormatting(PasswordTextBox);
                return false;
            }

            if(!IsStrongPassword(PasswordTextBox.Password))
            {
                PasswordErrorTextBlock.Text = "Password must contain at least 8 characters including at least one uppercase letter, one lowercase letter, and one number";
                SetErrorPasswordBoxFormatting(PasswordTextBox);
                return false;
            }
            return true;
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

        private bool IsStrongPassword(string password)
        {
            if(password.Length < 8) return false; // length >= 8
            if (!password.Any(c => char.IsUpper(c))) return false; // must include upper case letter
            if (!password.Any(c => char.IsLower(c))) return false; // must include lower case letter
            if (!password.Any(c => char.IsDigit(c))) return false; // must include a number

            return true;
        }


        #endregion        
    }
}
