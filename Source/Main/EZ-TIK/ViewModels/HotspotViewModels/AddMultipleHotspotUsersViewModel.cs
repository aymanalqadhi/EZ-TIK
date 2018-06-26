using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MaterialDesignThemes.Wpf.Transitions;
using Prism.Commands;
using System.Text.RegularExpressions;
using EZ_TIK.Annotations;
using EZ_TIK.Events;
using EZ_TIK.Models;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class AddMultipleHotspotUsersViewModel : ValidatedBindableBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="hotspotClient"></param>
        public AddMultipleHotspotUsersViewModel(IEventAggregator eventAggregator, IHotspotClient hotspotClient)
        {
            UsernameAndPasswordViewModel = new AddMultipleUsersUsernameAndPasswordViewModel();
            ProfileAndLimitsViewModel = new AddMultipleHotspotUsersProfileAndLimitsViewModel();

            ((DelegateCommand)UsernameAndPasswordViewModel.NextCommand).RaiseCanExecuteChanged();

            ProfileAndLimitsViewModel.FinishCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<AddMultipleHotspotUsersEvent>().Publish(this);
                DialogHost.CloseDialogCommand.Execute(null, null);
            });

            ProfileAndLimitsViewModel.RefreshProfilesCommand = new DelegateCommand(async () => ProfileAndLimitsViewModel.Profiles = (await hotspotClient.LoadAllProfilesAsync()).Select(p => p.Name));
            ProfileAndLimitsViewModel.RefreshProfilesCommand.Execute(null);
        }

        /// <summary>
        /// An instance of <see cref="AddMultipleUsersUsernameAndPasswordViewModel"/>
        /// </summary>
        public AddMultipleUsersUsernameAndPasswordViewModel UsernameAndPasswordViewModel { get; set; }

        /// <summary>
        /// An instance of <see cref="AddMultipleHotspotUsersProfileAndLimitsViewModel"/>
        /// </summary>
        public AddMultipleHotspotUsersProfileAndLimitsViewModel ProfileAndLimitsViewModel { get; set; }

        /// <summary>
        /// THe Profile and limitations viewmodel class
        /// </summary>

        public class AddMultipleHotspotUsersProfileAndLimitsViewModel : BindableBase
        {

            #region Private Fields

            /// <summary>
            /// The validity to use for the generated users ( in days )
            /// </summary>
            private string _validity;

            /// <summary>
            /// The price to be used for generating vouchers
            /// </summary>
            private string _price;

            /// <summary>
            /// The amount of the bytes to be used as limit for the upload
            /// </summary>
            private string _downloadAmount;

            /// <summary>
            /// The amount of the bytes to be used as limit for the download
            /// </summary>
            private string _uploadAmount;

            /// <summary>
            /// The days of the time-limt that used for the generated users
            /// </summary>
            private string _timeLimitDays;

            /// <summary>
            /// The hours of the time-limt that used for the generated users
            /// </summary>
            private string _timeLimitHours;

            /// <summary>
            /// The minutes of the time-limt that used for the generated users
            /// </summary>
            private string _timeLimitMinutes;

            #endregion

            #region Public Properties

            /// <summary>
            /// The list of the user profiles
            /// </summary>
            public IEnumerable<string> Profiles { get; set; }

            /// <summary>
            /// The selected profile index
            /// </summary>
            public short SelectedProfileIndex { get; set; }

            /// <summary>
            /// The validity to use for the generated users ( in days )
            /// </summary>
            public string Validity
            {
                get => _validity; set => SetIfNumeric(ref _validity, value);
            }

            /// <summary>
            /// The price to be used for generating vouchers
            /// </summary>
            public string Price
            {
                get => _price; set => SetIfNumeric(ref _price, value);
            }

            /// <summary>
            /// True if the user has enabled the bandwidth limit
            /// </summary>
            public bool HasBandwidthLimits { get; set; }

            /// <summary>
            /// The unit of the download limit ( KB - MB - GB )
            /// </summary>
            public byte DownloadUnitType { get; set; }

            /// <summary>
            /// The amount of the bytes to be used as limit for the download
            /// </summary>
            public string DownloadAmount
            {
                get => _downloadAmount; set => SetIfNumeric(ref _downloadAmount, value);
            }

            /// <summary>
            /// The unit of the upload limit ( KB - MB - GB )
            /// </summary>
            public byte UploadUnitType { get; set; }

            /// <summary>
            /// The amount of the bytes to be used as limit for the upload
            /// </summary>
            public string UploadAmount
            {
                get => _uploadAmount; set => SetIfNumeric(ref _uploadAmount, value);
            }

            /// <summary>
            /// True if the user has enabled the time limit
            /// </summary>
            public bool HasTimeLimits { get; set; }

            /// <summary>
            /// The days of the time-limt that used for the generated users
            /// </summary>
            public string TimeLimitDays
            {
                get => _timeLimitDays; set => SetIfNumeric(ref _timeLimitDays, value);
            }

            /// <summary>
            /// The hours of the time-limt that used for the generated users
            /// </summary>
            public string TimeLimitHours
            {
                get => _timeLimitHours; set => SetIfNumeric(ref _timeLimitHours, value);
            }

            /// <summary>
            /// The minutes of the time-limt that used for the generated users
            /// </summary>
            public string TimeLimitMinutes
            {
                get => _timeLimitMinutes; set => SetIfNumeric(ref _timeLimitMinutes, value);
            }

            #endregion

            #region Commands

            /// <summary>
            /// A Command to finish the adding operation
            /// </summary>
            public ICommand FinishCommand { get; set; }

            /// <summary>
            /// A Command to refresh the profiles list
            /// </summary>
            public ICommand RefreshProfilesCommand { get; set; }

            #endregion

            #region Private Helper Methods

            /// <summary>
            /// Set the field if the new value is in a numeric shape
            /// </summary>
            /// <param name="source">The filed to set its value</param>
            /// <param name="newValue">The new value to be set into the field</param>
            private void SetIfNumeric(ref string source, string newValue)
            {
                if (Regex.IsMatch(newValue, @"^\d*$"))
                    SetProperty(ref source, newValue);
            }

            #endregion
        }
    }
}