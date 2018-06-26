using System.Globalization;
using System.Windows.Controls;
using EZ_TIK.Parsers;
using Prism.Mvvm;
using tik4net.Objects.Ip.Hotspot;
using System.Windows.Input;

namespace EZ_TIK.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="HotspotUser"/>
    /// </summary>
    public class HotspotUserProfileViewModel : BindableBase
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="profile"></param>
        public HotspotUserProfileViewModel(HotspotUserProfile profile)
        {
            UserProfileModel = profile;
            Name = profile.Name;
            
            if(string.IsNullOrEmpty(UserProfileModel.RateLimit?.Trim()))
            {
                DownloadSpeed = UploadSpeed = "N/A";
                return;
            }
            else
            {
                var rate = UserProfileModel.RateLimit.Split('/');
                DownloadSpeed = rate[0];
                UploadSpeed = rate.Length > 1 ? rate[1] : rate[0];
            }
        }

        #endregion 

        #region Public Properties

        /// <summary>
        /// The model of this view model
        /// </summary>
        public HotspotUserProfile UserProfileModel { get; set; }

        /// <summary>
        /// The id of the profile in hex number
        /// </summary>
        public string Id => UserProfileModel.Id;

        /// <summary>
        /// The name of the profile
        /// </summary>
        public string Name
        {
            get => UserProfileModel.Name; set
            {
                if (UserProfileModel.Name == value) return;
                UserProfileModel.Name = value; RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The download traffic limit of the profile
        /// </summary>
        public string DownloadSpeed { get; private set; }

        /// <summary>
        /// The download traffic limit of the profile
        /// </summary>
        public string UploadSpeed { get; private set; }

        /// <summary>
        /// The time that account can saty alive when using this profile
        /// </summary>
        public string KeepAliveTimeout => MikrotikTime.Parse(UserProfileModel.KeepaliveTimeout)?.ToString() ?? "Not set";

        /// <summary>
        /// The time that account can stay before login again when using this profile
        /// </summary>
        public string SessionTimeout => MikrotikTime.Parse(UserProfileModel.SessionTimeout)?.ToString() ?? "Not set";

        /// <summary>
        /// Gets the first char of the profile name
        /// </summary>
        public char FirstCharOfName => Name?.ToUpper()[0] ?? '?';

        /// <summary>
        /// Gets the shared users
        /// </summary>
        public string SharedUsers => string.IsNullOrEmpty(UserProfileModel.SharedUsers) || UserProfileModel.SharedUsers?.Trim() == "unlimited" ? "Unlimited" : UserProfileModel.SharedUsers;

        /// <summary>
        /// Gets or sets if the profile is currently selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets wether this profile is the default profile or not
        /// </summary>
        public bool IsDefaultProfile => Name == "default";

        #endregion

        #region Commands

        /// <summary>
        /// A Command to fire when the selection state changes
        /// </summary>
        public ICommand SelectionChangedCommand { get; set; }

        /// <summary>
        /// A Command to delete the profiled
        /// </summary>
        public ICommand DeleteProfileCommand { get; set; }

        #endregion
    }
}
