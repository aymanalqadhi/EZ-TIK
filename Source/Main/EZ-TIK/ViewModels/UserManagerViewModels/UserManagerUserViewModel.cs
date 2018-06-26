using System;
using System.Globalization;
using EZ_TIK.Models;
using EZ_TIK.Models.Interfaces;
using EZ_TIK.Parsers;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class UserManagerUserViewModel : BindableBase, IUser
    {
        public UserManagerUserViewModel(UserManagerUser user, UserManagerProfile profile, UserManagerProfileLimitation limitation = null)
        {
            UserModel = user;
            ProfileModel = profile;
            ProfileLimitationModel = limitation ?? new UserManagerProfileLimitation();
        }

        /// <summary>
        /// THe Limits of the profile that used by this account
        /// </summary>
        public UserManagerProfileLimitation ProfileLimitationModel { get; set; }

        /// <summary>
        /// The actual model of this class
        /// </summary>
        public UserManagerUser UserModel { get; set; }

        /// <summary>
        /// The Model of the profile
        /// </summary>
        public UserManagerProfile ProfileModel { get; set; }

        /// <summary>
        /// THe ID of the user
        /// </summary>
        public long Id => long.Parse(UserModel.Id.Trim('*'), NumberStyles.HexNumber);

        /// <summary>
        /// The username of the user
        /// </summary>
        public string Username
        {
            get => UserModel.Name; set
            {
                UserModel.Name = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password
        {
            get => UserModel.Password; set
            {
                UserModel.Password = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// The owner of the user
        /// </summary>
        public string Customer
        {
            get => UserModel.Customer; set
            {
                UserModel.Customer = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The status of the user
        /// </summary>
        public bool IsDisabled
        {
            get => UserModel.Disabled; set
            {
                UserModel.Disabled = value;
                RaisePropertyChanged();
            }
        }


        /// <summary>
        /// The max numbers of the users that can use this account at the same time
        /// </summary>
        public string SharedUsers
        {
            get => UserModel.SharedUsers; set
            {
                UserModel.SharedUsers = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// True if the user is selected at the view
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// The price of this account
        /// </summary>
        public int? Price
        {
            get => ProfileModel?.Price; set
            {
                if (UserModel != null)
                {
                    ProfileModel.Price = value.GetValueOrDefault();
                    RaisePropertyChanged();
                }

            }
        }

        /// <summary>
        /// THe validity of this account from the first use
        /// </summary>
        public string Validity
        {
            get => ProfileModel?.Validity; set
            {
                ProfileModel.Validity = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The name of the profiles of the user
        /// </summary>
        public string ActualProfile
        {
            get => ProfileModel == null ? "Not Activated" : ProfileModel.Name; set
            {
                if (ProfileModel == null) return;
                ProfileModel.Name = value;
            }
        }

        /// <summary>
        /// The allowed time from the first start
        /// </summary>
        public string TimeLimit
        {
            get => ByteSize.FromBytes(ProfileLimitationModel.UploadLimit).ToString(); set => ProfileLimitationModel.UptimeLimit = value;
        }

        /// <summary>
        /// THe amount of download allowed
        /// </summary>
        public string DownloadLimit
        {
            get => ProfileLimitationModel != null && ProfileLimitationModel.DownloadLimit > 0 ? ByteSize.FromBytes(ProfileLimitationModel.DownloadLimit).ToString() : "Unlimited"; set
            {
                var bytes = ByteSize.MinValue;
                ByteSize.TryParse(value, out bytes);
                ProfileLimitationModel.DownloadLimit = (long)bytes.Bytes;
            }
        }

        /// <summary>
        /// THe amount of upload allowed
        /// </summary>
        public string UploadLimit
        {
            get => ProfileLimitationModel != null && ProfileLimitationModel.UploadLimit > 0 ? ByteSize.FromBytes(ProfileLimitationModel.UploadLimit).ToString() : "Unlimited"; set
            {
                var bytes = ByteSize.MinValue;
                ByteSize.TryParse(value, out bytes);
                ProfileLimitationModel.UploadLimit = (long)bytes.Bytes;
            }
        }

        /// <summary>
        /// THe limitation name of the user's profile
        /// </summary>
        public string Limitation { get; set; }

        /// <summary>
        /// The comment of the user
        /// </summary>
        public string Comment
        {
            get => UserModel.Comment; set
            {
                UserModel.Comment = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The last time the user has used the account
        /// </summary>
        public string LastSeen => UserModel.LastSeen;

        /// <summary>
        /// The used time by the user
        /// </summary>
        public string UptimeUsed => UserModel.UptimeUsed;

        /// <summary>
        /// The used download by the user
        /// </summary>
        public string DownloadUsed => UserModel.DownloadUsed;

        /// <summary>
        /// The ip address of the user of this account
        /// </summary>
        public string IpAddress => UserModel.IpAddress;
    }

}
