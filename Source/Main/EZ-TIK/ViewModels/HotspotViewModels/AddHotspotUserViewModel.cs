using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using EZ_TIK.Events;
using EZ_TIK.Models;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using tik4net.Objects;
using tik4net.Objects.Ip.Hotspot;

namespace EZ_TIK.ViewModels
{
    public class AddHotspotUserViewModel : BindableBase
    {
        private readonly HotspotUserViewModel _userToEdit;

        public AddHotspotUserViewModel(IEventAggregator eventAggregator, IHotspotClient hotspotClient)
        {
            IsUpdate = false;

            #region Init Commands

            // A Command to refresh the list of the user profiles and select the first one if the list count is greater than 0
            BasicViewModel.RefreshProfilesCommand = new DelegateCommand(async () =>
            {
                // Load profiles
                BasicViewModel.Profiles = (await hotspotClient.LoadAllProfilesAsync()).Select(p => p.Name).ToList();

                // Select the first one
                if (BasicViewModel.Profiles.Count > 0) BasicViewModel.SelectedProfileIndex = _userToEdit != null ? BasicViewModel.Profiles.IndexOf(_userToEdit.Profile) : 0;
            });

            // A Command to add the built user
            LimitsViewModel.FinishCommand = new DelegateCommand(() =>
            {
                // Init a new user
                var user = _userToEdit?.UserModel ?? new HotspotUser();

                user.Name = BasicViewModel.Username;
                user.Password = BasicViewModel.Password;

                // Calculate the profile value
                user.Profile = BasicViewModel.SelectedProfileIndex >= 0 ? (BasicViewModel.Profiles.Count - 1 >= BasicViewModel.SelectedProfileIndex ? BasicViewModel.Profiles[BasicViewModel.SelectedProfileIndex] : "default") : "default";


                // TODO:
                // Do a record add for the user notes

                // Set bandwidth limit
                if (LimitsViewModel.HasBandwidthLimits)
                {
                    // Array of the common sizes
                    var mul = new long[] { 1024, 1024 * 1024, 1024 * 1024 * 1024 };

                    // Set the bandwidth limits
                    user.LimitBytesOut = (!string.IsNullOrEmpty(LimitsViewModel.DownloadAmount) ? int.Parse(LimitsViewModel.DownloadAmount) : 0) * mul[LimitsViewModel.DownloadType];
                    user.LimitBytesIn =  (!string.IsNullOrEmpty(LimitsViewModel.UploadAmount) ? int.Parse(LimitsViewModel.UploadAmount) : 0) * mul[LimitsViewModel.UploadType];
                }

                // Set timelimit if the user has set one
                if (LimitsViewModel.HasTimeLimits)
                {
                    // Calculate the timelimt using timespan and replace the time span dot with mikrotik "d " symbole
                    var timeLimit = new TimeSpan(!string.IsNullOrEmpty(LimitsViewModel.TimeLimitDays) ? int.Parse(LimitsViewModel.TimeLimitDays) : 0,
                                                 !string.IsNullOrEmpty(LimitsViewModel.TimeLimitDays) ? int.Parse(LimitsViewModel.TimeLimitDays) : 0,
                                                 !string.IsNullOrEmpty(LimitsViewModel.TimeLimitDays) ? int.Parse(LimitsViewModel.TimeLimitDays) : 0
                                                 );

                    user.LimitUptime = timeLimit.ToString().Replace(".", "d ");
                }

                // Raise the add event
                User = user;
                eventAggregator.GetEvent<AddHotspotUserEvent>().Publish(this);

                // Close the dialog
                DialogHost.CloseDialogCommand.Execute(null, null);
            });

            #endregion

            BasicViewModel.RefreshProfilesCommand.Execute(null);
        }

        public AddHotspotUserViewModel(IEventAggregator eventAggregator, IHotspotClient hotspotClient, HotspotUserViewModel userToEdit) : this(eventAggregator, hotspotClient)
        {
            if (userToEdit?.Username != null) IsUpdate = true;
            else return;

            OriginalName = userToEdit.Username;

            _userToEdit = userToEdit;

            // Set Basic info
            BasicViewModel.Username = userToEdit.Username;
            BasicViewModel.Password = userToEdit.Password;

            // Set the bandwidth limit if there is
            if (userToEdit.LimitBytesOut.Bytes > 0 || userToEdit.LimitBytesIn.Bytes > 0)
            {
                LimitsViewModel.HasBandwidthLimits = true;

                // Setting LimitBytesOut
                if (Math.Floor(userToEdit.LimitBytesOut.GigaBytes) > 0)
                {
                    LimitsViewModel.DownloadType = 2;
                    LimitsViewModel.DownloadAmount = userToEdit.LimitBytesOut.GigaBytes.ToString(CultureInfo.InvariantCulture);
                }
                else if (Math.Floor(userToEdit.LimitBytesOut.MegaBytes) > 0)
                {
                    LimitsViewModel.DownloadType = 1;
                    LimitsViewModel.DownloadAmount = userToEdit.LimitBytesOut.MegaBytes.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    LimitsViewModel.DownloadType = 0;
                    LimitsViewModel.DownloadAmount = userToEdit.LimitBytesOut.KiloBytes.ToString(CultureInfo.InvariantCulture);
                }

                // Setting LimitBytesIn
                if (Math.Floor(userToEdit.LimitBytesIn.GigaBytes) > 0)
                {
                    LimitsViewModel.UploadType = 2;
                    LimitsViewModel.UploadAmount = userToEdit.LimitBytesIn.GigaBytes.ToString(CultureInfo.InvariantCulture);
                }
                else if (Math.Floor(userToEdit.LimitBytesIn.MegaBytes) > 0)
                {
                    LimitsViewModel.UploadType = 1;
                    LimitsViewModel.UploadAmount = userToEdit.LimitBytesIn.MegaBytes.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    LimitsViewModel.UploadType = 0;
                    LimitsViewModel.UploadAmount = userToEdit.LimitBytesIn.KiloBytes.ToString(CultureInfo.InvariantCulture);
                }

            }

            // Set time limits
            LimitsViewModel.HasTimeLimits = !string.IsNullOrEmpty(userToEdit.LimitUptime);

            var time = Parsers.MikrotikTime.Parse(userToEdit.LimitUptime);

            if (time != null)
            {
                LimitsViewModel.HasTimeLimits = true;
                LimitsViewModel.TimeLimitDays = time.Days.ToString();
                LimitsViewModel.TimeLimitHours = time.Hours.ToString();
                LimitsViewModel.TimeLimitMinutes = time.Mintues.ToString();
            }

            LimitsViewModel.Price = userToEdit.Price != null && userToEdit.Price != "free" && Regex.IsMatch(userToEdit.Price, @"^\d+$") ? userToEdit.Price : string.Empty; 

            if (userToEdit.Validity > -1)
                LimitsViewModel.Validity = userToEdit.Validity.ToString();
        }

        /// <summary>
        /// First page ViewModel
        /// </summary>
        public AddHotspotUserBasicViewModel BasicViewModel { get; set; } = new AddHotspotUserBasicViewModel();

        /// <summary>
        /// Second Page ViewModel
        /// </summary>
        public AddHotspotUserLimitsViewModel LimitsViewModel { get; set; } = new AddHotspotUserLimitsViewModel();

        /// <summary>
        /// True if the dialog is in the update mode
        /// </summary>
        public bool IsUpdate { get; private set; }

        /// <summary>
        /// THe user is being updated ( if in update mode )
        /// </summary>

        public HotspotUser User { get; private set; }

        /// <summary>
        /// The original name of the user is being updated ( if in update mode )
        /// </summary>

        public string OriginalName { get; private set; }
    }

    #region Child ViewModels

    /// <summary>
    /// ViewModel for the first page of the main view which used for setting basic info for the user
    /// </summary>
    public class AddHotspotUserBasicViewModel : ValidatedBindableBase
    {
        #region Private members

        private string _username;

        #endregion

        #region Constrcutors

        /// <summary>
        /// Default constrcutor
        /// </summary>
        public AddHotspotUserBasicViewModel()
        {
            #region Init Commands

            NextCommand = new DelegateCommand(() =>
            {
                if (!string.IsNullOrEmpty(Username.Trim())) Transitioner.MoveNextCommand.Execute(null, null);
            }, () => !string.IsNullOrEmpty(Username?.Trim()));

            #endregion
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// THe username of the user ( REQUIRED )
        /// </summary>
        [Required]
        public string Username
        {
            get => _username; set
            {
                SetProperty(ref _username, value);
                ((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// THe password of the user
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The selected profile index
        /// </summary>
        public int SelectedProfileIndex { get; set; }

        /// <summary>
        /// an optional notes for the users
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// The names of the hotspot user profiles
        /// </summary>
        public List<string> Profiles { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// A Command to navigate to the next view
        /// </summary>
        public ICommand NextCommand { get; set; }

        /// <summary>
        /// A Command to refresh the profiles list
        /// </summary>
        public ICommand RefreshProfilesCommand { get; set; }

        #endregion
    }

    /// <summary>
    /// ViewModel for the second page of the main view which used for setting limits of the user
    /// </summary>
    public class AddHotspotUserLimitsViewModel : ValidatedBindableBase
    {
        #region Private Members

        private string _validity;
        private string _timeLimitMinutes;
        private string _timeLimitHours;
        private string _timeLimitDays;
        private string _downloadAmount;
        private string _uploadAmount;
        private string _price;

        #endregion

        #region Public Properties

        /// <summary>
        /// True if the user has added bandwidth limits to the user
        /// </summary>
        public bool HasBandwidthLimits { get; set; }

        /// <summary>
        /// True if the user has added time limits to the user
        /// </summary>
        public bool HasTimeLimits { get; set; }

        /// <summary>
        /// The allowed upload amount
        /// </summary>
        public string UploadAmount
        {
            get => _uploadAmount; set
            {
                if (Regex.IsMatch(value, @"^\d+$")) _uploadAmount = value;
            }
        }

        /// <summary>
        /// The unit used to set the upload limit
        /// </summary>
        public byte UploadType { get; set; } = 1;

        /// <summary>
        /// The allowed download amount
        /// </summary>
        public string DownloadAmount
        {
            get => _downloadAmount; set
            {
                if (Regex.IsMatch(value, @"^\d+$")) _downloadAmount = value;
            }
        }

        /// <summary>
        /// The unit used to set the download limit
        /// </summary>
        public byte DownloadType { get; set; } = 1;

        /// <summary>
        /// The days of the time limit
        /// </summary>
        public string TimeLimitDays
        {
            get => _timeLimitDays; set
            {
                if (Regex.IsMatch(value, @"^\d+$")) _timeLimitDays = value;
            }
        }

        /// <summary>
        /// The hours of the time limit
        /// </summary>
        public string TimeLimitHours
        {
            get => _timeLimitHours; set
            {
                if (Regex.IsMatch(value, @"^\d+$")) _timeLimitHours = value;
            }
        }

        /// <summary>
        /// The minutes of the time limi
        /// </summary>
        public string TimeLimitMinutes
        {
            get => _timeLimitMinutes; set
            {
                if (Regex.IsMatch(value, @"^\d+$")) _timeLimitMinutes = value;
            }
        }

        /// <summary>
        /// The validity of the user from the first login ( in days )
        /// </summary>
        public string Validity
        {
            get => _validity; set
            {
                if (Regex.IsMatch(value, @"^\d+$")) _validity = value;
            }
        }

        /// <summary>
        /// The validity of the user from the first login ( in days )
        /// </summary>
        public string Price
        {
            get => _price; set
            {
                if (Regex.IsMatch(value, @"^\d+$")) _price = value;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// A Command to navigate to the next view
        /// </summary>
        public ICommand FinishCommand { get; set; }

        #endregion
    }

    #endregion
}