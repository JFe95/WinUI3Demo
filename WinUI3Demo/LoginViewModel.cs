using System.ComponentModel;
using System.Linq;

namespace WinUI3Demo
{
    public class LoginViewModel :INotifyPropertyChanged
    {
        #region Fields

        private string _email;
        private string _password;
        private string _emailErrorText;
        private string _passwordErrorText;

        #endregion

        #region Properties

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Password 
        { 
            get => _password; 
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string EmailErrorText
        {
            get => _emailErrorText; 
            set
            {
                _emailErrorText = value;
                OnPropertyChanged(nameof(EmailErrorText));
            }
        }

        public string PasswordErrorText
        {
            get => _passwordErrorText; 
            set
            {
                _passwordErrorText = value;
                OnPropertyChanged(nameof(PasswordErrorText));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ValidateLogin() { return ValidateEmail() & ValidatePassword(); }

        private bool ValidateEmail()
        {
            if (string.IsNullOrEmpty(Email))
            {
                EmailErrorText = "Email is a required field";
                return false;
            }
            return true;
        }

        private bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(Password))
            {
                PasswordErrorText = "Password is a required field";
                return false;
            }

            if (!IsStrongPassword(Password))
            {
                PasswordErrorText = "Password must contain at least 8 characters including at least one uppercase letter, one lowercase letter, and one number";
                return false;
            }
            return true;
        }

        private static bool IsStrongPassword(string password)
        {
            if (password.Length < 8) return false; // length >= 8
            if (!password.Any(c => char.IsUpper(c))) return false; // must include upper case letter
            if (!password.Any(c => char.IsLower(c))) return false; // must include lower case letter
            if (!password.Any(c => char.IsDigit(c))) return false; // must include a number

            return true;
        }

        #endregion
    }
}
