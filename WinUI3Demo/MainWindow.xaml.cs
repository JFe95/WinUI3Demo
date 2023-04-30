
using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Linq;
using Windows.UI.Popups;
using Microsoft.UI.Xaml.Controls;
using Windows.Devices.Enumeration;
using System;

namespace WinUI3Demo
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow _apw;
        private OverlappedPresenter _presenter;


        public MainWindow()
        {
            this.InitializeComponent();
            this.ExtendsContentIntoTitleBar = true;
            GetAppWindow();
            _apw.Resize(new Windows.Graphics.SizeInt32 { Width = 500, Height = 570 });
            _presenter.IsMaximizable = false;
            _presenter.IsMinimizable = false;
            _presenter.IsResizable = false;

        }

        public void GetAppWindow()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _apw = AppWindow.GetFromWindowId(myWndId);
            _presenter = _apw.Presenter as OverlappedPresenter;
        }

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (ValidateLogin()) DisplaySuccessDialog();
        }

        private async void DisplaySuccessDialog()
        {
            var dialog = new ContentDialog
            {
                Title = "Success",
                Content = "Account created",
                CloseButtonText = "OK"              
            };
            //set the XamlRoot property
            dialog.XamlRoot = MainPanel.XamlRoot;

            await dialog.ShowAsync();
            Application.Current.Exit();

        }

        private void OnEmailTextChanged(object sender, RoutedEventArgs e)
        {
            UsernameErrorTextBlock.Text = string.Empty;
        }

        private void OnPasswordTextChanged(object sender, RoutedEventArgs e)
        {
            PasswordErrorTextBlock.Text = string.Empty;
        }

        private bool ValidateLogin() { return ValidateEmail() & ValidatePassword(); }

        private bool ValidateEmail()
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameErrorTextBlock.Text = "Email cannot be empty";
                return false;
            }
            return true;
        }

        private bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                PasswordErrorTextBlock.Text = "Password cannot be empty";
                return false;
            }
            if(!IsStrongPassword(PasswordTextBox.Password))
            {
                PasswordErrorTextBlock.Text = "Password must contain at least 8 characters including at least one uppercase letter, one lowercase letter, and one number";
            }
            return true;
        }

        private bool IsStrongPassword(string password)
        {
            if(password.Length < 8) return false; // length >= 8
            if (!password.Any(c => char.IsUpper(c))) return false; // must include upper case letter
            if (!password.Any(c => char.IsLower(c))) return false; // must include lower case letter
            if (!password.Any(c => char.IsDigit(c))) return false; // must include a number

            return true;
        }
    }
}
