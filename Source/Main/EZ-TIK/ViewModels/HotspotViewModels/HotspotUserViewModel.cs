using System.Globalization;
using System.Text.RegularExpressions;
using EZ_TIK.Models.Interfaces;
using EZ_TIK.Parsers;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Mvvm;
using tik4net.Objects.Ip.Hotspot;

namespace EZ_TIK.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="HotspotUser"/>
    /// </summary>
    public class HotspotUserViewModel : BindableBase, IUser
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="user"></param>
        public HotspotUserViewModel(HotspotUser user)
        {
            UserModel = user;

            if (user.Email != null)
            {
                Validity = Regex.IsMatch(user.Email.Trim(), @"^(\d+)\@") ? int.Parse(Regex.Match(user.Email.Trim(), @"^(\d+)\@").Groups[1].Value) : -1;
                Price = Regex.IsMatch(user.Email.Trim(), @"^\w+?\@(\d+)?\.\w+$") ? Regex.Match(user.Email.Trim(), @"^\w+?\@(\d+)?\.\w+$").Groups[1].Value : "free";
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The model of this viewmodel
        /// </summary>
        public HotspotUser UserModel { get; private set; }

        /// <summary>
        /// Gets or sets the ID of the user
        /// </summary>
        public long Id => long.Parse(UserModel.Id.Trim('*'), NumberStyles.HexNumber);

        /// <summary>
        /// Gets or sets t1 username of the user
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
        /// Gets or sets the password of the user
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
        /// Gets or sets the profile of the user
        /// </summary>
        public string Profile
        {
            get => UserModel.Profile; set
            {
                UserModel.Profile = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the MacAddress of the user
        /// </summary>
        public string MacAddress
        {
            get => UserModel.MacAddress; set
            {
                UserModel.MacAddress = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the IpAddress of the user
        /// </summary>
        public string IpAddress
        {
            get => UserModel.Address; set
            {
                UserModel.Address = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Comment of the user
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
        /// Gets or sets the Email of the user
        /// </summary>
        public string Email
        {
            get => UserModel.Email; set
            {
                UserModel.Email = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Download limit of the user
        /// </summary>
        public ByteSize LimitBytesOut
        {
            get => ByteSize.FromBytes(UserModel.LimitBytesOut); set => UserModel.LimitBytesOut = (long)value.Bytes;
        }

        /// <summary>
        /// Gets or sets the Upload limit of the user
        /// </summary>
        public ByteSize LimitBytesIn
        {
            get => ByteSize.FromBytes(UserModel.LimitBytesIn); set => UserModel.LimitBytesIn = (long)value.Bytes;
        }

        /// <summary>
        /// Gets or sets the Download & Upload limit of the user
        /// </summary>
        public ByteSize LimitBytesTotal
        {
            get => ByteSize.FromBytes(UserModel.LimitBytesTotal); set => UserModel.LimitBytesTotal = (long)value.Bytes;
        }

        /// <summary>
        /// Gets or sets the time limit of the user
        /// </summary>
        public string LimitUptime
        {
            get => UserModel.LimitUptime; set
            {
                UserModel.LimitUptime = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Downloaded bytes by user
        /// </summary>
        public ByteSize BytesOut => ByteSize.FromBytes(UserModel.BytesOut);

        /// <summary>
        /// Gets or sets the Uploaded bytes by user
        /// </summary>
        public ByteSize BytesIn => ByteSize.FromBytes(UserModel.BytesIn);

        /// <summary>
        /// Gets or sets the used time by user
        /// </summary>
        public string UpTime => UserModel.Uptime;

        /// <summary>
        /// True if the user is disabled in the server
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
        /// The server that this user is on
        /// </summary>
        public string Server
        {
            get => UserModel.Server; set
            {
                UserModel.Server = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if the user is currently selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// The validity of the user in days
        /// </summary>
        public int Validity { get; set; }

        /// <summary>
        /// The price of the account ( if presits )
        /// </summary>
        public string Price { get; set; }

        #endregion

        #region Public Methods

        public void SetModel(HotspotUser user)
        {
            if (user == null) return;

            UserModel = user;
            GetType().GetProperties().ForEach(p => RaisePropertyChanged(p.Name));
        }
        #endregion
    }
}