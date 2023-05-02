using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml.Media;

namespace WinUI3Demo
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;
        private OverlappedPresenter _presenter;
        private SolidColorBrush _placeholderColorBrush;

        public SolidColorBrush PlaceHolderColorBrush
        {
            get
            {
                return _placeholderColorBrush;
            }
            set 
            { 
                _placeholderColorBrush = value; 
            }
        } 

        #region Constructor

        public MainWindow()
        {
            this.InitializeComponent();
            GetAppWindow();
            _appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 500, Height = 570 });
            _presenter.IsResizable = false;
            _presenter.SetBorderAndTitleBar(false, false);
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

        #endregion

        #region Methods

        public void GetAppWindow()
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(myWndId);
            _presenter = _appWindow.Presenter as OverlappedPresenter;
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
        private void SetErrorPasswordBoxFormatting(PasswordBox textBox)
        {
            var ErrorColor = (Windows.UI.Color)Application.Current.Resources["ErrorColor"];

            textBox.Style = (Style)Application.Current.Resources["PasswordBoxErrorStyle"];
            var placeholderColorBrush = textBox.Resources["TextControlPlaceholderForeground"] as SolidColorBrush;
            placeholderColorBrush.Color = ErrorColor;
            placeholderColorBrush = textBox.Resources["TextControlPlaceholderForegroundFocused"] as SolidColorBrush;
            placeholderColorBrush.Color = ErrorColor;
            placeholderColorBrush = textBox.Resources["TextControlPlaceholderForegroundPointerOver"] as SolidColorBrush;
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
