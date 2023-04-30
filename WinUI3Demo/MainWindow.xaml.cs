
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using System;

namespace WinUI3Demo
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;
        private OverlappedPresenter _presenter;


        public MainWindow()
        {
            this.InitializeComponent();
            GetAppWindow();
            _appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 500, Height = 570 });
            _presenter.IsMaximizable = false;
            _presenter.IsMinimizable = false;
            _presenter.IsResizable = false;
            _presenter.SetBorderAndTitleBar(false, false);
        }

        public void GetAppWindow()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(myWndId);
            _presenter = _appWindow.Presenter as OverlappedPresenter;
        }

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (ValidateLogin()) 
                DisplaySuccessDialog();
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

        private void OnEmailTextChanged(object sender, RoutedEventArgs e)
        {
            SetDefaultTextBoxFormatting((Control)sender);
            UsernameErrorTextBlock.Text = string.Empty;
        }

        private void OnPasswordTextChanged(object sender, RoutedEventArgs e)
        {
            SetDefaultTextBoxFormatting((Control)sender);
            PasswordErrorTextBlock.Text = string.Empty;
        }

        private bool ValidateLogin() { return ValidateEmail() & ValidatePassword(); }

        private bool ValidateEmail()
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameErrorTextBlock.Text = "Email is a required field";
                SetDefaultTextBoxFormatting(UsernameTextBox);
                return false;
            }
            return true;
        }

        private bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                PasswordErrorTextBlock.Text = "Password is a required field";
                SetErrorTextBoxFormatting(PasswordTextBox);
                return false;
            }

            if(!IsStrongPassword(PasswordTextBox.Password))
            {
                PasswordErrorTextBlock.Text = "Password must contain at least 8 characters including at least one uppercase letter, one lowercase letter, and one number";
                SetErrorTextBoxFormatting(PasswordTextBox);
            }
            return true;
        }

        private void SetDefaultTextBoxFormatting(Control textBox)
        {
            textBox.Style = (Style)Application.Current.Resources["TextBoxStyle"];
        }

        private void SetErrorTextBoxFormatting(Control textBox)
        {
            textBox.Style = (Style)Application.Current.Resources["TextBoxErrorStyle"];
        }

        private bool IsStrongPassword(string password)
        {
            if(password.Length < 8) return false; // length >= 8
            if (!password.Any(c => char.IsUpper(c))) return false; // must include upper case letter
            if (!password.Any(c => char.IsLower(c))) return false; // must include lower case letter
            if (!password.Any(c => char.IsDigit(c))) return false; // must include a number

            return true;
        }

        private void Button_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }
    }
}
