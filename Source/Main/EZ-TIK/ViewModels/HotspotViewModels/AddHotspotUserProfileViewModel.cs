using EZ_TIK.Events;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Windows.Input;
using tik4net.Objects.Ip.Hotspot;

namespace EZ_TIK.ViewModels.HotspotViewModels
{
    public class AddHotspotUserProfileViewModel : ValidatedBindableBase
    {
        #region Private Fields

        /// <summary>
        /// <see cref="SharedUsers"/> private variable
        /// </summary>
        private string _sharedUsers;

        /// <summary>
        /// <see cref="DownloadSpeed"/> private variable
        /// </summary>
        private string _downloadSpeed;

        /// <summary>
        /// <see cref="UploadSpeed"/> private variable
        /// </summary>
        private string _uploadSpeed;

        /// <summary>
        /// Use this if in edit mode
        /// </summary>
        private HotspotUserProfile _toEdit;

        /// <summary>
        /// A Service to manage events
        /// </summary>
        private IEventAggregator _eventAggregator;
        private string _name;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AddHotspotUserProfileViewModel(IEventAggregator eventAggregator, HotspotUserProfile toEdit = null)
        {
            _eventAggregator = eventAggregator;
            _toEdit = toEdit;

            // Call helper methods
            InitCommands();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A Helper method to init viewmodel commands
        /// </summary>
        private void InitCommands()
        {
            // Init the finish command
            OkayCommand = new DelegateCommand(() =>
            {
                if (HasErrors) return;

                var rate = string.Empty;

                if (!string.IsNullOrEmpty(DownloadSpeed) && !string.IsNullOrEmpty(UploadSpeed))
                    rate = $"{DownloadSpeed}/{UploadSpeed}";
                else if (!string.IsNullOrEmpty(DownloadSpeed))
                    rate = DownloadSpeed;
                else
                    rate = UploadSpeed;

                var profile = new HotspotUserProfile
                {
                    Name = Name,
                    SharedUsers = SharedUsers,
                    RateLimit = rate
                };

                _eventAggregator.GetEvent<AddHotspotUserProfileEvent>().Publish(new HotspotUserProfileViewModel(profile));
                DialogHost.CloseDialogCommand.Execute(null, null);
            }, () => CanAdd);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name field value
        /// </summary>
        [Required]
        public string Name
        {
            get => _name; set
            {
                SetProperty(ref _name, value);
                (OkayCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The shared users field value
        /// </summary>
        public string SharedUsers
        {
            get => _sharedUsers; set { if (!Regex.IsMatch(value, @"^\d*$")) return; SetProperty(ref _sharedUsers, value); }
        }

        /// <summary>
        /// The download rate limit field value
        /// </summary>
        public string DownloadSpeed
        {
            get => _downloadSpeed; set { if (!Regex.IsMatch(value, @"^\d*(k|m|g)?$")) return; SetProperty(ref _downloadSpeed, value); }
        }

        /// <summary>
        /// The upload rate limit field value
        /// </summary>
        public string UploadSpeed
        {
            get => _uploadSpeed; set { if (!Regex.IsMatch(value, @"^\d*(k|m|g)?$")) return; SetProperty(ref _uploadSpeed, value); }
        }

        /// <summary>
        /// Can't execute Ok command if this false
        /// </summary>
        public bool CanAdd => !string.IsNullOrEmpty(Name?.Trim());

        #endregion

        #region Commands

        /// <summary>
        /// A Command to finish the creating/editing operation
        /// </summary>
        public ICommand OkayCommand { get; set; }

        #endregion
    }
}
