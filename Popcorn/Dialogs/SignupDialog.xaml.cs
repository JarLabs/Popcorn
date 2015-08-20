﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using Popcorn.Helpers;

namespace Popcorn.Dialogs
{
    public class SignupDialogSettings : MetroDialogSettings
    {
        private readonly string _defaultTitle = LocalizationProviderHelper.GetLocalizedValue<string>("SignupLabel");
        private readonly string _defaultMessage = LocalizationProviderHelper.GetLocalizedValue<string>("SigninMessage");
        private readonly string _defaultUsernameWatermark = LocalizationProviderHelper.GetLocalizedValue<string>("UserNameWatermark");
        private readonly string _defaultFirstNameWatermark = LocalizationProviderHelper.GetLocalizedValue<string>("FirstNameWatermark");
        private readonly string _defaultLastNameWatermark = LocalizationProviderHelper.GetLocalizedValue<string>("LastNameWatermark");
        private readonly string _defaultEmailWatermark = LocalizationProviderHelper.GetLocalizedValue<string>("EmailWatermark");
        private readonly string _defaultPasswordWatermark = LocalizationProviderHelper.GetLocalizedValue<string>("PasswordWatermark");
        private readonly string _defaultConfirmPasswordWatermark = LocalizationProviderHelper.GetLocalizedValue<string>("ConfirmPasswordWatermark");
        private const bool DefaultEnablePasswordPreview = false;

        public SignupDialogSettings()
        {
            Title = _defaultTitle;
            Message = _defaultMessage;
            UsernameWatermark = _defaultUsernameWatermark;
            FirstNameWatermark = _defaultFirstNameWatermark;
            LastNameWatermark = _defaultLastNameWatermark;
            EmailWatermark = _defaultEmailWatermark;
            PasswordWatermark = _defaultPasswordWatermark;
            ConfirmPasswordWatermark = _defaultConfirmPasswordWatermark;
            EnablePasswordPreview = DefaultEnablePasswordPreview;
        }

        public string Title { get; }

        public string Message { get; }

        public string SignupButtonText { get; }

        public string UsernameWatermark { get; }

        public string FirstNameWatermark { get; }

        public string LastNameWatermark { get; }

        public string EmailWatermark { get; }

        public string PasswordWatermark { get; }

        public string ConfirmPasswordWatermark { get; }

        public bool EnablePasswordPreview { get; }
    }

    public class SignupDialogData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public partial class SignupDialog : INotifyDataErrorInfo
    {
        internal SignupDialog(SignupDialogSettings settings)
        {
            InitializeComponent();
            Message = settings.Message;
            Title = settings.Title;
            UsernameWatermark = settings.UsernameWatermark;
            FirstNameWatermark = settings.FirstNameWatermark;
            LastNameWatermark = settings.LastNameWatermark;
            EmailWatermark = settings.EmailWatermark;
            PasswordWatermark = settings.PasswordWatermark;
            ConfirmPasswordWatermark = settings.ConfirmPasswordWatermark;
        }

        internal Task<SignupDialogData> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Focus();
                if (string.IsNullOrEmpty(PART_TextBox.Text))
                {
                    PART_TextBox.Focus();
                }
                else
                {
                    PART_TextBox2.Focus();
                }
            }));

            TaskCompletionSource<SignupDialogData> tcs = new TaskCompletionSource<SignupDialogData>();

            RoutedEventHandler signupHandler = null;
            KeyEventHandler signupKeyHandler = null;

            RoutedEventHandler cancelHandler = null;
            KeyEventHandler cancelKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanUpHandlers();
                tcs.TrySetResult(null);
            });

            cleanUpHandlers = () => {
                PART_TextBox.KeyDown -= signupKeyHandler;
                PART_TextBox2.KeyDown -= signupKeyHandler;
                PART_TextBox3.KeyDown -= signupKeyHandler;

                KeyDown -= escapeKeyHandler;

                PART_SignupButton.Click -= signupHandler;
                PART_CancelButton.Click -= cancelHandler;

                PART_SignupButton.KeyDown -= signupKeyHandler;
                PART_CancelButton.KeyDown -= cancelKeyHandler;

                cancellationTokenRegistration.Dispose();
            };

            escapeKeyHandler = (sender, e) =>
            {
                if (e.Key != Key.Escape) return;
                cleanUpHandlers();

                tcs.TrySetResult(null);
            };

            signupKeyHandler = (sender, e) =>
            {
                if (e.Key != Key.Enter) return;
                var isValid = IsUsernameValid(Username)
                              && IsEmailValid(Email)
                              && IsFirstNameValid(FirstName)
                              && IsLastNameValid(LastName)
                              && IsPasswordValid(Password)
                              && IsConfirmPasswordValid(ConfirmPassword);
                if (isValid)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(new SignupDialogData
                    {
                        Username = Username,
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Password = Password,
                        ConfirmPassword = ConfirmPassword
                    });
                }
            };

            signupHandler = (sender, e) =>
            {
                var isValid = IsUsernameValid(Username)
                               && IsEmailValid(Email)
                               && IsFirstNameValid(FirstName)
                               && IsLastNameValid(LastName)
                               && IsPasswordValid(Password)
                               && IsConfirmPasswordValid(ConfirmPassword);
                if (!isValid) return;
                cleanUpHandlers();

                tcs.TrySetResult(new SignupDialogData
                {
                    Username = Username,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword
                });

                e.Handled = true;
            };

            cancelKeyHandler = (sender, e) =>
            {
                if (e.Key != Key.Enter) return;
                cleanUpHandlers();
                tcs.TrySetResult(null);
            };

            cancelHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(null);

                e.Handled = true;
            };

            PART_SignupButton.KeyDown += signupKeyHandler;
            PART_CancelButton.KeyDown += cancelKeyHandler;

            PART_TextBox.KeyDown += cancelKeyHandler;
            PART_TextBox2.KeyDown += cancelKeyHandler;
            PART_TextBox3.KeyDown += cancelKeyHandler;

            KeyDown += escapeKeyHandler;

            PART_SignupButton.Click += signupHandler;
            PART_CancelButton.Click += cancelHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            var settings = DialogSettings as SignupDialogSettings;
            if (settings != null && settings.EnablePasswordPreview)
            {
                var win8MetroPasswordStyle = FindResource("Win8MetroPasswordBox") as Style;
                if (win8MetroPasswordStyle != null)
                {
                    PART_TextBox2.Style = win8MetroPasswordStyle;
                    PART_TextBox3.Style = win8MetroPasswordStyle;
                }
            }

            switch (DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    PART_SignupButton.Style = FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox3.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameWatermarkProperty = DependencyProperty.Register("UsernameWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty FirstNameProperty = DependencyProperty.Register("FirstName", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty FirstNameWatermarkProperty = DependencyProperty.Register("FirstNameWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty LastNameProperty = DependencyProperty.Register("LastName", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty LastNameWatermarkProperty = DependencyProperty.Register("LastNameWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty EmailProperty = DependencyProperty.Register("Email", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty EmailWatermarkProperty = DependencyProperty.Register("EmailWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ConfirmPasswordProperty = DependencyProperty.Register("ConfirmPassword", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register("PasswordWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ConfirmPasswordWatermarkProperty = DependencyProperty.Register("ConfirmPasswordWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public string FirstName
        {
            get { return (string)GetValue(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public string FirstNameWatermark
        {
            get { return (string)GetValue(FirstNameWatermarkProperty); }
            set { SetValue(FirstNameWatermarkProperty, value); }
        }

        public string LastName
        {
            get { return (string)GetValue(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public string LastNameWatermark
        {
            get { return (string)GetValue(LastNameWatermarkProperty); }
            set { SetValue(LastNameWatermarkProperty, value); }
        }

        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        public string EmailWatermark
        {
            get { return (string)GetValue(EmailWatermarkProperty); }
            set { SetValue(EmailWatermarkProperty, value); }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public string ConfirmPassword
        {
            get { return (string)GetValue(ConfirmPasswordProperty); }
            set { SetValue(ConfirmPasswordProperty, value); }
        }

        public string UsernameWatermark
        {
            get { return (string)GetValue(UsernameWatermarkProperty); }
            set { SetValue(UsernameWatermarkProperty, value); }
        }

        public string PasswordWatermark
        {
            get { return (string)GetValue(PasswordWatermarkProperty); }
            set { SetValue(PasswordWatermarkProperty, value); }
        }

        public string ConfirmPasswordWatermark
        {
            get { return (string)GetValue(ConfirmPasswordWatermarkProperty); }
            set { SetValue(ConfirmPasswordWatermarkProperty, value); }
        }

        // Validates the Username property, updating the errors collection as needed.
        private bool IsUsernameValid(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("Username", UsernameEmptyError, false);
                isValid = false;
            }
            else RemoveError("Username", UsernameEmptyError);

            return isValid;
        }

        // Validates the FirstName property, updating the errors collection as needed.
        private bool IsFirstNameValid(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("FirstName", FirstNameEmptyError, false);
                isValid = false;
            }
            else RemoveError("FirstName", FirstNameEmptyError);

            return isValid;
        }

        // Validates the LastName property, updating the errors collection as needed.
        private bool IsLastNameValid(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("LastName", LastNameEmptyError, false);
                isValid = false;
            }
            else RemoveError("LastName", LastNameEmptyError);

            return isValid;
        }

        // Validates the Email property, updating the errors collection as needed.
        private bool IsEmailValid(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("Email", EmailEmptyError, false);
                isValid = false;
            }
            else RemoveError("Email", EmailEmptyError);

            return isValid;
        }

        // Validates the Password property, updating the errors collection as needed.
        private bool IsPasswordValid(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("Password", PasswordEmptyError, false);
                isValid = false;
            }
            else RemoveError("Password", PasswordEmptyError);

            if (!string.IsNullOrEmpty(value) && value.Length < 6) AddError("Password", PasswordFormatError, true);
            else RemoveError("Password", PasswordFormatError);

            return isValid;
        }

        // Validates the ConfirmPassword property, updating the errors collection as needed.
        private bool IsConfirmPasswordValid(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value) || value != Password)
            {
                AddError("ConfirmPassword", ConfirmPasswordMismatchError, false);
                isValid = false;
            }
            else RemoveError("ConfirmPassword", ConfirmPasswordMismatchError);

            return isValid;
        }

        private readonly Dictionary<string, List<string>> _errors =
            new Dictionary<string, List<string>>();
        private const string UsernameEmptyError = "Username must be filled.";
        private const string EmailEmptyError = "Email must be filled.";
        private const string FirstNameEmptyError = "First name must be filled.";
        private const string LastNameEmptyError = "Last name must be filled.";
        private const string PasswordEmptyError = "Password must be filled.";
        private const string PasswordFormatError = "Password must contain at least 6 characters.";
        private const string ConfirmPasswordMismatchError = "Passwords must match.";

        // Adds the specified error to the errors collection if it is not 
        // already present, inserting it in the first position if isWarning is 
        // false. Raises the ErrorsChanged event if the collection changes. 
        private void AddError(string propertyName, string error, bool isWarning)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (_errors[propertyName].Contains(error)) return;
            if (isWarning) _errors[propertyName].Add(error);
            else _errors[propertyName].Insert(0, error);
            RaiseErrorsChanged(propertyName);
        }

        // Removes the specified error from the errors collection if it is
        // present. Raises the ErrorsChanged event if the collection changes.
        private void RemoveError(string propertyName, string error)
        {
            if (_errors.ContainsKey(propertyName) &&
                _errors[propertyName].Contains(error))
            {
                _errors[propertyName].Remove(error);
                if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #region INotifyDataErrorInfo Members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) ||
                !_errors.ContainsKey(propertyName))
                return null;
            return _errors[propertyName];
        }

        public bool HasErrors => _errors.Count > 0;

        #endregion
    }
}
