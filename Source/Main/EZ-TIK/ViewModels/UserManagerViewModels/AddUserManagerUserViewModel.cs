using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using EZ_TIK.Events;
using EZ_TIK.Models;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class AddUserManagerUserViewModel : BindableBase
    {
        /// <summary>
        /// Default construtor
        /// </summary>
        public AddUserManagerUserViewModel(IUserManagerClient userManagerClient, IEventAggregator eventAggregator)
        {
            BasicViewModel = new AddUserManagerUserBasicViewModel(userManagerClient);
            ProfileViewModel = new AddUserManagerUserProfileViewModel(userManagerClient);

            #region Init Commands

            ProfileViewModel.FinishCommand = new DelegateCommand(() =>
            {
                eventAggregator.GetEvent<AddUserManagerUserEvent>().Publish(this);
                DialogHost.CloseDialogCommand.Execute(null, null);
            }, () => !ProfileViewModel.ActivateUserNow || ProfileViewModel.SelectedProfileIndex > -1);

            #endregion

            BasicViewModel.RefreshCustomersCommand.Execute(null);
        }

        /// <summary>
        /// The basic view model
        /// </summary>
        public AddUserManagerUserBasicViewModel BasicViewModel { get; set; }

        /// <summary>
        /// Activation viewmodel
        /// </summary>
        public AddUserManagerUserProfileViewModel ProfileViewModel { get; set; }


    }

    /// <summary>
    /// BasicView ViewModel Class
    /// </summary>
    public class AddUserManagerUserBasicViewModel : BindableBase
    {
        private string _sharedUsers;

        public AddUserManagerUserBasicViewModel(IUserManagerClient userManagerClient)
        {
            // Init Next Command
            NextCommand =
                new DelegateCommand(() => Transitioner.MoveNextCommand.Execute(null, null),
                    () => !string.IsNullOrEmpty(Username) && SelectedCustomerIndex > -1).ObservesProperty(
                    () => SelectedCustomerIndex).ObservesProperty(() => Username);

            // Init Refresh Customers command
            RefreshCustomersCommand =
                new DelegateCommand(
                    async () =>
                        Customers =
                            new ObservableCollection<string>(
                                (await userManagerClient.LoadAllCustomersAsync()).Select(c => c.Login)));
        }

        #region Public Properties

        /// <summary>
        /// THe list of the user manager customers
        /// </summary>
        public ObservableCollection<string> Customers { get; set; }

        /// <summary>
        /// Selected customer index
        /// </summary>
        public int SelectedCustomerIndex { get; set; }

        /// <summary>
        /// The username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// A comment for the user
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The max users that can use this account at a time
        /// </summary>
        public string SharedUsers
        {
            get => _sharedUsers; set
            {
                if (Regex.IsMatch(value, @"^\d+$")) _sharedUsers = value;
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// A Command to navigate to the next view
        /// </summary>
        public ICommand NextCommand { get; set; }

        /// <summary>
        /// A Command to refresh customers list
        /// </summary>
        public ICommand RefreshCustomersCommand { get; set; }

        /// <summary>
        /// A Command to show AddUserManagerCustomer dialog
        /// </summary>
        public ICommand AddCustomerCommand { get; set; }

        #endregion
    }

    /// <summary>
    /// Profile ViewModel Class
    /// </summary>
    public class AddUserManagerUserProfileViewModel : BindableBase
    {
        private int _selectedProfileIndex = -1;
        private bool _activateUserNow;

        public AddUserManagerUserProfileViewModel(IUserManagerClient userManagerClient)
        {
            Profiles = new ObservableCollection<UserManagerProfileViewModel>();

            #region Init Commands

            RefreshProfilesCommand = new DelegateCommand(async () =>
            {
                var limits = await userManagerClient.LoadAllProfileLimitationsAsync();
                var profLimits = await userManagerClient.LoadAllProfileProfileLimitationsAsync();

                Profiles = new ObservableCollection<UserManagerProfileViewModel>((await userManagerClient.LoadAllProfilesAsync()).Select(p =>
                {
                    var pl = profLimits.SingleOrDefault(pf => pf.Profile == p.Name);
                    if (pl == null) return new UserManagerProfileViewModel(p);

                    var limit = limits.SingleOrDefault(l => l.Name == pl.Limitation);
                    return limit != null ? new UserManagerProfileViewModel(p, limit) : new UserManagerProfileViewModel(p);
                }));

                if (Profiles.Any()) SelectedProfileIndex = 0;
                ((DelegateCommand)FinishCommand).RaiseCanExecuteChanged();
            });

            #endregion

            RefreshProfilesCommand.Execute(null);
        }

        /// <summary>
        /// True if the user wants to activate the account now
        /// </summary>
        public bool ActivateUserNow
        {
            get => _activateUserNow; set
            {
                _activateUserNow = value;
                RaisePropertyChanged();
                ((DelegateCommand)FinishCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The list of the usermanager profiles
        /// </summary>
        public IEnumerable<UserManagerProfileViewModel> Profiles { get; set; }

        /// <summary>
        /// The profiles to be showen
        /// </summary>
        public ObservableCollection<string> ProfilesList => new ObservableCollection<string>(Profiles.Select(p => p.Name));

        /// <summary>
        /// The selected profile index
        /// </summary>
        public int SelectedProfileIndex
        {
            get => _selectedProfileIndex; set
            {
                _selectedProfileIndex = value;
                SelectedProfile = Profiles.ToList()[value];
                ((DelegateCommand)FinishCommand).RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The selected profile ViewModel
        /// </summary>
        public UserManagerProfileViewModel SelectedProfile { get; set; }

        #region Commands

        /// <summary>
        /// A Command to refresh the profiles list
        /// </summary>
        public ICommand RefreshProfilesCommand { get; set; }

        /// <summary>
        /// A Command to finish the proccess
        /// </summary>
        public ICommand FinishCommand { get; set; }

        #endregion

    }
}
