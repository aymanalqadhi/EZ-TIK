using EZ_TIK.Models;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace EZ_TIK.ViewModels
{
    public class UserManagerProfileViewModel : BindableBase
    {
        #region Private Fields

        /// <summary>
        /// The backing-field of <see cref="IsSelected"/> property
        /// </summary>
        private bool _isSelected;

        #endregion

        #region Constructors

        public UserManagerProfileViewModel(UserManagerProfile profile, UserManagerProfileLimitation limitation = null)
        {
            ProfileModel = profile;
            LimitationModel = limitation;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The model of the profile
        /// </summary>
        public UserManagerProfile ProfileModel { get; set; }

        /// <summary>
        /// The model object of the limits of this profile ( null if not set )
        /// </summary>
        public UserManagerProfileLimitation LimitationModel { get; set; }

        /// <summary>
        /// The name of the profile
        /// </summary>
        public string Name
        {
            get => ProfileModel.Name;
            set
            {
                ProfileModel.Name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The owner customer of the profile
        /// </summary>
        public string Owner
        {
            get => ProfileModel.Onwer;
            set
            {
                ProfileModel.Onwer = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Limitation setting name of the profile
        /// </summary>
        public string Limitation
        {
            get => LimitationModel?.Name ?? "Not Set";
            set
            {
                LimitationModel.Name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// THe price of accounts of this profile
        /// </summary>
        public int Price
        {
            get => ProfileModel.Price;
            set
            {
                ProfileModel.Price = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// THe downlosd limit of the profile
        /// </summary>
        public long? DownloadLimit
        {
            get => LimitationModel?.DownloadLimit;
            set
            {
                LimitationModel.DownloadLimit = value.GetValueOrDefault();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// THe upload limit in bytes of the profile
        /// </summary>
        public long? UploadLimit
        {
            get => LimitationModel?.UploadLimit;
            set
            {
                LimitationModel.UploadLimit = value.GetValueOrDefault();
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The TIme Limit of the profile
        /// </summary>
        public string TimeLimit
        {
            get => LimitationModel?.UptimeLimit;
            set
            {
                LimitationModel.UptimeLimit = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The validity of the profile
        /// </summary>
        public string Validity
        {
            get => ProfileModel.Validity;
            set
            {
                ProfileModel.Validity = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The Maximum number of users that can use an account with this profile at the same time 
        /// </summary>
        public string OverrideSharedUsers
        {
            get => ProfileModel.OverrideSharedUsers;
            set
            {
                ProfileModel.OverrideSharedUsers = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The selection state of this profile in the view
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                SetProperty(ref _isSelected, value);
                SelectionChangeCommand.Execute(null);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// A Command to invoke on selection change
        /// </summary>
        public ICommand SelectionChangeCommand { get; set; }

        #endregion

    }
}
