using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using EZ_TIK.Annotations;
using EZ_TIK.Models.Interfaces;
using MaterialDesignThemes.Wpf.Transitions;
using Prism.Commands;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{

    /// <summary>
    /// THe Username and password viewmodel class
    /// </summary>
    public class AddMultipleUsersUsernameAndPasswordViewModel : BindableBase
    {
        #region Private Members

        private string _usernameLength;
        private string _passwordLength;
        private bool _isUsernameRandomlyGenerated = true;
        private bool _isPasswordTheSameAsUsername;
        private string _usernameStartNumber;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddMultipleUsersUsernameAndPasswordViewModel()
        {
            NextCommand = new DelegateCommand(() => Transitioner.MoveNextCommand.Execute(null, null), () => CanNavigate);
            ((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// True if the user can navigate to the next view
        /// </summary>
        public bool CanNavigate
        {
            get
            {
                var canNevigate = (IsUsernameRandomlyGenerated && !string.IsNullOrEmpty(UsernameLength)) || !IsUsernameRandomlyGenerated;
                canNevigate = canNevigate && ((!IsPasswordTheSameAsUsername && !string.IsNullOrEmpty(PasswordLength)) || IsPasswordTheSameAsUsername);

                return canNevigate;
            }
        }

        /// <summary>
        /// True if the user has selected the random generating mode
        /// </summary>
        public bool IsUsernameRandomlyGenerated
        {
            get => _isUsernameRandomlyGenerated; set
            {
                _isUsernameRandomlyGenerated = value;
                ((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// True if the user wants to set the password to be the same as the username
        /// </summary>
        public bool IsPasswordTheSameAsUsername
        {
            get => _isPasswordTheSameAsUsername; set
            {
                _isPasswordTheSameAsUsername = value;
                ((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The minimum length of the username
        /// </summary>
        public string UsernameLength
        {
            get => _usernameLength; set
            {
                if (Regex.IsMatch(value, @"^\d*$") && value.Length < 3) _usernameLength = value;
                ((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The minimum length of the password
        /// </summary>
        public string PasswordLength
        {
            get => _passwordLength; set
            {
                if (Regex.IsMatch(value, @"^\d*$") && value.Length < 3) _passwordLength = value;

                ((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The type of the username characters ( Letters - Numbers - Mixed )
        /// </summary>
        public byte UsernameCharactersType { get; set; }

        /// <summary>
        /// The type of the Password characters ( Letters - Numbers - Mixed )
        /// </summary>
        public byte PasswordCharactersType { get; set; }

        /// <summary>
        /// The characters to be at the begining of the uniformly generated username
        /// </summary>
        public string UsernameStartCharacters { get; set; }

        /// <summary>
        /// The number to start generating username from
        /// </summary>
        [NotNull]
        public string UsernameStartNumber
        {
            get => _usernameStartNumber; set
            {
                if (Regex.IsMatch(value, @"^\d*$")) _usernameStartNumber = value;
            }
        }

        /// <summary>
        /// The character that the user doesn't want to be in the password
        /// </summary>
        public string PasswordUnusedCharacters { get; set; }

        /// <summary>
        /// True if the Password Letters only selected
        /// </summary>
        public bool IsPasswordLettersOnly { get; set; } = true;

        /// <summary>
        /// True if the Password Numbers only selected
        /// </summary>
        public bool IsPasswordNumbersOnly { get; set; }

        /// <summary>
        /// True if the Password mixed selected
        /// </summary>
        public bool IsPasswordMixed { get; set; }

        /// <summary>
        /// True if the Username Letters only selected
        /// </summary>
        public bool IsUsernameLettersOnly { get; set; } = true;

        /// <summary>
        /// True if the Username Numbers only selected
        /// </summary>
        public bool IsUsernameNumbersOnly { get; set; }

        /// <summary>
        /// True if the Username mixed selected
        /// </summary>
        public bool IsUsernameMixed { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// A Command to navigate to the next view
        /// </summary>
        public ICommand NextCommand { get; set; }

        #endregion
    }
}
