using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EZ_TIK.Events;
using EZ_TIK.Models;
using EZ_TIK.Utils;
using EZ_TIK.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProgressDialog = EZ_TIK.Resources.ProgressDialog;

namespace EZ_TIK.ViewModels
{
    public class UserManagerUsersViewModel : BindableBase
    {

        #region Private Members

        private readonly IUserManagerClient _userManagerClient;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserManagerUsersViewModel(IUserManagerClient userManagerClient, IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _userManagerClient = userManagerClient;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            // Init public properties
            Users = new ObservableCollection<UserManagerUserViewModel>();


            // Init Commands
            InitCommands();

            // Subscribe events
            SubscribeEvents();

            // Refresh the users list
            RefreshCommand.Execute(null);
        }

        #endregion

        #region Private Helpers

        private void InitCommands()
        {
            #region Refresh Command

            // Init Refresh Command 
            RefreshCommand = new DelegateCommand<DataGrid>(async dg =>
            {
                await DialogHost.Show(new ProgressDialog(), "RootDialog", async (s, e) =>
                {
                    var profiles = await _userManagerClient.LoadAllProfilesAsync();
                    var limits = await _userManagerClient.LoadAllProfileLimitationsAsync();
                    var profileLimits = await _userManagerClient.LoadAllProfileProfileLimitationsAsync();

                    // fetch users from server
                    var users = (await _userManagerClient.LoadAllUsersAsync()).ToList().Select(u =>
                    {
                        var user = new UserManagerUserViewModel(u, profiles.SingleOrDefault(p => p.Name == u.ActualProfile));

                        var profileLimit = profileLimits.SingleOrDefault(pl => pl.Profile == user.ActualProfile);
                        if (profileLimit == null) return user;

                        var limit = limits.SingleOrDefault(l => l.Name == profileLimit.Limitation);
                        user.ProfileLimitationModel = limit;

                        return user;
                    });


                    // Update user list
                    Users.Clear();
                    Users.AddRange(users);

                    e.Session.Close();

                    // update the users count and search command
                    RaisePropertyChanged(nameof(UsersCount));
                    RaisePropertyChanged(nameof(SelectedUsersCount));
                    RaisePropertyChanged(nameof(HasSelectedUsers));
                    ((DelegateCommand<DataGrid>)SearchCommand).RaiseCanExecuteChanged();

                }, null);

            });

            #endregion

            #region Remove Command

            // Init the remove command
            RemoveCommand = new DelegateCommand(async () =>
            {
                // Get selected users
                var selected = Users.Where(u => u.IsSelected).Select(u => u.UserModel).ToList();
                var selectedCount = selected.Count;

                // return if the selected users are 0
                if (selectedCount <= 0) return;

                // Do a single deletion
                if (selectedCount == 1)
                {
                    Action deleteAction = null;
                    deleteAction = async () =>
                    {
                        var done = await _userManagerClient.RemoveUserAsync(Users.Single(u => u.IsSelected).UserModel);
                        if (done) Users.Remove(Users.SingleOrDefault(u => u.IsSelected));

                        // Inform the user
                        _eventAggregator.GetEvent<NotificationEvent>().Publish(
                            done
                                ? new NotificationEventArgs("Deleted User Successfully!", "UNDO", null, new SideNotificationViewModel("UserManager Users", "Deleted User Successfully!"))
                                : new NotificationEventArgs("Failed in deleting user", "RETRY",
                                    () => deleteAction.Invoke(), new SideNotificationViewModel("UserManager Users", "Deleted User Successfully!"))
                        );

                        RaisePropertyChanged(nameof(UsersCount));
                    };

                    deleteAction.Invoke();
                    return;
                }

                // Show Confirm Message and return if the user has canceled
                var confirm = await _dialogService.ShowMessageAsync("Confirm",
                    $"Are sure you want to delete selected items ( {selected.Count} items ) ?",
                    MessageDialogStyle.AffirmativeAndNegative);
                if (confirm == MessageDialogResult.Negative) return;

                var loading = await _dialogService.ShowProgressAsync("Deleting",
                    $"Deleting Item - from {SelectedUsersCount} items...");
                loading.SetCancelable(true);

                var i = 0;
                for (; i < selectedCount; i++)
                {
                    // break if canceled
                    if (loading.IsCanceled) break;

                    // set progress of the progress dialog
                    loading.SetMessage($"Deleting Item {i} from {selectedCount} items...");
                    loading.SetProgress((double)i / selectedCount);

                    // continue if failed
                    if (!await _userManagerClient.RemoveUserAsync(selected[i])) continue;

                    // Update the UI
                    var item = Users.SingleOrDefault(u => u.Username == selected[i].Name);
                    if (item != null) Users.Remove(item);
                }

                // Close the progress dialog
                await loading.CloseAsync();

                // Show Notifcation
                _eventAggregator.GetEvent<NotificationEvent>()
                    .Publish(new NotificationEventArgs($"Deleted {i} items successfully!", null, new SideNotificationViewModel("UserManager Users", $"Deleted {i + 1} items successfully!")));

                RaisePropertyChanged(nameof(UsersCount));
            });

            #endregion

            #region Toggle Command

            ToggleCommand = new DelegateCommand<bool?>(async newValue =>
            {
                // return if there is no selected users
                if (!HasSelectedUsers) return;

                // fetch selected users
                var selected = await Task.Run(() => Users.Where(u => u.IsSelected && u.IsDisabled != newValue.GetValueOrDefault()).Select(u => Users.IndexOf(u)).ToList());
                if (selected.Count == 0) return;

                // Do A Single action if the selected users count is 0
                if (selected.Count == 1)
                {
                    // set the disabled value
                    Users[selected[0]].IsDisabled = newValue.GetValueOrDefault();

                    // Notification message string
                    string message;

                    // do modification operation
                    if (await _userManagerClient.UpdateUserAsync(Users[selected[0]].UserModel))
                    {
                        // set the message to success if the change is successed
                        message = $"The user \"{Users[selected[0]].Username}\" has been {(Users[selected[0]].IsDisabled ? "disabled" : "enabled")} successfully!";
                    }
                    else
                    {
                        // set the message to fail if the operation has failed
                        message = $"Can't {(Users[selected[0]].IsDisabled ? "disable" : "enable")} user!";

                        // set the disabled value back to the value that it was
                        Users[selected[0]].IsDisabled = !newValue.GetValueOrDefault();
                    }

                    // Show notification about the proccess
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs(message, null, new SideNotificationViewModel("Hotspot Users", message)));

                    // Update the UI and return
                    UsersSelectionChanged();
                    return;
                }

                /* Do Multiple Action */

                // calculate the action string to be used in messages
                var actionStr = newValue.GetValueOrDefault() ? "Disabling" : "Enabling";

                // Init the loading dialog
                var loading = await _dialogService.ShowProgressAsync("Toggling Users", $"{actionStr} users...");
                loading.SetCancelable(true);

                int i = 0, fail = 0;
                for (; i < selected.Count; i++)
                {
                    Users[selected[i]].IsDisabled = newValue.GetValueOrDefault();

                    // return if the user has canceled the operation
                    if (loading.IsCanceled) break;

                    // Update the progress dialog
                    loading.SetMessage($"{actionStr} User \"{Users[selected[i]].Username}\" ...");
                    loading.SetProgress((double)i / selected.Count);

                    // DO the changing task and do the dependecies
                    if (!await _userManagerClient.UpdateUserAsync(Users[selected[i]].UserModel))
                    {
                        Users[selected[i]].IsDisabled = !newValue.GetValueOrDefault();
                        fail++;
                    }
                    else
                    {
                        Users[selected[i]].IsDisabled = newValue.GetValueOrDefault();
                    }
                }

                // close the progress dialog
                await loading.CloseAsync();

                // Show a notification about the operation
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs($"{i - fail + 1} Users have been {(newValue.GetValueOrDefault() ? "disabled" : "enabled")} successfully!", null, new SideNotificationViewModel("Hotspot Users", $"{i - fail + 1} Users have been {(newValue.GetValueOrDefault() ? "disabled" : "enabled")} successfully!")));

                // Update UI
                UsersSelectionChanged();
            });

            #endregion

            #region Search Command

            // A Command to find a user from the search box
            SearchCommand = new DelegateCommand<DataGrid>(async dg =>
            {
                if (string.IsNullOrEmpty(SearchText?.Trim())) return;

                await Task.Run(() =>
                {
                    // found items list
                    var found = new List<UserManagerUserViewModel>();

                    // do a parallel search in background
                    Parallel.ForEach(Users, user =>
                    {
                        // if entry matched add it to the list
                        if (user != null && (user.Username.Contains(SearchText) || user.Password.Contains(SearchText)))
                        {
                            found.Add(user);
                        }
                    });

                    // Deselect all items
                    Users.ForEach(u => u.IsSelected = false);

                    // Set found items as selected
                    foreach (var user in found)
                    {
                        if (user == null) continue;
                        user.IsSelected = true;

                        // Scroll to the last item
                        if (user == found.Last()) Application.Current.Invoke(() => dg.ScrollIntoView(user));
                    }

                    // Show notification about the task status
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs(SelectedUsersCount > 0 ? $"Found {SelectedUsersCount} Matches!" : "No Matches Found!"));

                    // Update the UI
                    RaisePropertyChanged(nameof(SelectedUsersCount));
                    RaisePropertyChanged(nameof(HasSelectedUsers));

                });

            }, dg => UsersCount > 0);

            #endregion

            #region Add Usermanager User Command

            AddUserManagerUserCommand = new DelegateCommand(async () => await DialogHost.Show(new AddUserManagerUserView()));

            #endregion

            #region Add Multiple Usermanager Users Command

            AddUsersCommand = new DelegateCommand(async () => await DialogHost.Show(new AddMultipleUserManagerUsersView()));

            #endregion
        }

        /// <summary>
        /// Subsribe to related events
        /// </summary>
        private void SubscribeEvents()
        {
            #region Add User Event

            _eventAggregator.GetEvent<AddUserManagerUserEvent>().Subscribe(async userVm =>
            {
                if (userVm == null) return;

                var user = new UserManagerUser
                {
                    Customer = userVm.BasicViewModel.Customers[userVm.BasicViewModel.SelectedCustomerIndex],
                    Name = userVm.BasicViewModel.Username,
                    Password = userVm.BasicViewModel.Password,
                    Comment = userVm.BasicViewModel.Comment,
                    SharedUsers = !string.IsNullOrEmpty(userVm.BasicViewModel.SharedUsers) ? userVm.BasicViewModel.SharedUsers : "unlimited"
                };

                var done = await _userManagerClient.AddUserAsync(user);
                var msg = done ? "User Add Successfully!" : "Fail Adding User!";
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs(msg, null, new SideNotificationViewModel("UserManager Users", msg)));

                if (done && userVm.ProfileViewModel.ActivateUserNow && !string.IsNullOrEmpty(userVm.ProfileViewModel.SelectedProfile?.Name))
                {
                    done = await _userManagerClient.ActivateUser(user, userVm.ProfileViewModel.SelectedProfile.Name);
                    msg = done ? "User Activated Successfully!" : "Fail Activating User!";
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs(msg, null, new SideNotificationViewModel("UserManager Users", msg)));
                }

                // Adds to the users list
                Users.Add(new UserManagerUserViewModel(user, userVm.ProfileViewModel.ActivateUserNow ? userVm.ProfileViewModel?.SelectedProfile?.ProfileModel : null, userVm.ProfileViewModel?.SelectedProfile?.LimitationModel));
            });

            #endregion

            #region Add Multiple Users Event

            _eventAggregator.GetEvent<AddMultipleUserManagerUsersEvent>().Subscribe(async usersVm =>
            {
                // Fetches the users count from the user 
                var countObj = await _dialogService.ShowInputAsync("Add UserManager Users", "Enter the count of the users you want to add");
                if (countObj == null) return;

                int count;
                if (!int.TryParse(countObj.ToString(), out count))
                {
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs("Invalid input!"));
                    return;
                }

                // Shows a progress dialog
                var dialog = await _dialogService.ShowProgressAsync("Adding users", $"Adding user - of {count}");
                dialog.SetCancelable(true);

                // Gets the customer name 
                var customer = usersVm.ProfileAndOwnerViewModel.Customers[usersVm.ProfileAndOwnerViewModel.SelectedCustomerIndex];

                // Created an empty list to contain the created users
                var created = new List<UserManagerUserViewModel>();

                // Gets if the user wants to activate the created users now
                var activateNow = usersVm.ProfileAndOwnerViewModel.ActivateUserNow;

                // Starts adding
                for (int i = 0, j = 0; i < count;)
                {
                    // Breaks if the user has canceled the operation
                    if (dialog.IsCanceled) break;

                    // Generates the username and the password
                    string name, password;
                    if (usersVm.UsernameAndPasswordViewModel.IsUsernameRandomlyGenerated)
                    {
                        var nameKind = usersVm.UsernameAndPasswordViewModel.IsUsernameLettersOnly
                            ? RandomStringKind.CharactersOnly
                            : usersVm.UsernameAndPasswordViewModel.IsUsernameNumbersOnly
                                ? RandomStringKind.NumbersOnly
                                : RandomStringKind.Mixed;
                        name = StringUtils.GetRandomString(nameKind,
                            int.Parse(usersVm.UsernameAndPasswordViewModel.UsernameLength));
                    }
                    else
                        name = $"{usersVm.UsernameAndPasswordViewModel.UsernameStartCharacters}{int.Parse(usersVm.UsernameAndPasswordViewModel.UsernameStartNumber) + j++}";

                    // Reloops if the generated username is exits
                    if (Users.Any(u => u.Username == name)) continue;

                    // Genrates the password
                    if (usersVm.UsernameAndPasswordViewModel.IsPasswordTheSameAsUsername) password = name;
                    else
                    {
                        var passKind = usersVm.UsernameAndPasswordViewModel.IsPasswordLettersOnly ? RandomStringKind.CharactersOnly : usersVm.UsernameAndPasswordViewModel.IsPasswordNumbersOnly ? RandomStringKind.NumbersOnly : RandomStringKind.Mixed;
                        password = StringUtils.GetRandomString(passKind, int.Parse(usersVm.UsernameAndPasswordViewModel.PasswordLength));
                    }

                    // Updates the progress dialog message
                    dialog.SetMessage($"Adding user ( {i + 1} : {name} ) of {count}");
                    dialog.SetProgress((double)i / count);

                    // Inits new user
                    var user = new UserManagerUser
                    {
                        Name = name,
                        Password = password,
                        Customer = "admin",
                        SharedUsers = !string.IsNullOrEmpty(usersVm.ProfileAndOwnerViewModel.SharedUsers.Trim()) ? usersVm.ProfileAndOwnerViewModel.SharedUsers : "unlimited"
                    };

                    // Adds it to the created list if successed on adding it to the server
                    if (await _userManagerClient.AddUserAsync(user))
                        created.Add(new UserManagerUserViewModel(user, null));

                    // Increases the counter if every thing is ok
                    i++;
                }

                // Activates the created users ( if user has choosed to )
                if (usersVm.ProfileAndOwnerViewModel.ActivateUserNow)
                {
                    // Gets the selected profile by the user
                    var profile = usersVm.ProfileAndOwnerViewModel.SelectedProfile;

                    // Skips if the user is null or its name is not valid
                    if (string.IsNullOrEmpty(profile?.Name.Trim())) goto skip;

                    // Updates the progress dialog message
                    dialog.SetTitle("Activating Users");

                    // Starts the activating proccess
                    for (var i = 0; i < created.Count; i++)
                    {
                        dialog.SetMessage($"Activating User \"{created[i].Username}\"...");
                        dialog.SetProgress((double)i / created.Count);

                        if (await _userManagerClient.ActivateUser(created[i].UserModel, profile.Name))
                            created[i].ProfileModel = profile.ProfileModel;
                    }
                }

                skip:
   
                // Closes the progress dialog
                await dialog.CloseAsync();

                // Adds created users to the view
                Users.AddRange(created);

                // Shows a summury notification
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs($"Added {created.Count} Users Successfully!", null, new SideNotificationViewModel("UserManager Users", $"Added {created.Count} Users Successfully!")));
                RaisePropertyChanged(nameof(UsersCount));
            });

            #endregion
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Users List fetched from the router
        /// </summary>
        public ObservableCollection<UserManagerUserViewModel> Users { get; set; }

        /// <summary>
        /// The count of users
        /// </summary>
        public int UsersCount => Users.Count;

        /// <summary>
        /// The count of selected users
        /// </summary>
        public int SelectedUsersCount => Users.Count(u => u.IsSelected);
        /// <summary>
        /// Gets true if all items <see cref="Users"/> are selected
        /// </summary>
        public bool IsAllSelected
        {
            get => Users.Count != 0 && Users.All(u => u.IsSelected); set
            {
                Users.ForEach(u => u.IsSelected = value);
                UsersSelectionChanged();
            }
        }

        /// <summary>
        /// True if there is a selected users
        /// </summary>
        public bool HasSelectedUsers => Users.Any(u => u.IsSelected);

        /// <summary>
        /// True if there is only one selected user
        /// </summary>
        public bool HasSingleSelectedUsers => Users.Count(u => u.IsSelected) == 1;

        /// <summary>
        /// The text used for searching user
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// The Index of the selected user
        /// </summary>
        public int SelectedUserIndex { get; set; } = -1;

        /// <summary>
        /// Boolean constants (True)
        /// </summary>
        public bool TrueValue => true;

        /// <summary>
        /// Boolean constants (False)
        /// </summary>
        public bool FalseValue => false;

        /// <summary>
        /// True if the users list has a disabled users
        /// </summary>
        public bool HasDisabledUsers => Users.Any(u => u.IsDisabled && u.IsSelected);

        /// <summary>
        /// True if the users list has a enabled users
        /// </summary>
        public bool HasEnabledUsers => Users.Any(u => !u.IsDisabled && u.IsSelected);

        #endregion

        #region Commands

        /// <summary>
        /// A Command To Refresh the list of the users
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        /// A Command to delete the selected users
        /// </summary>
        public ICommand RemoveCommand { get; set; }

        /// <summary>
        /// A Command to toggle the user
        /// </summary>
        public ICommand ToggleCommand { get; set; }

        /// <summary>
        /// A Command to be used to search for user
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// A Command to show add dialog to add a user
        /// </summary>
        public ICommand AddUserManagerUserCommand { get; set; }

        /// <summary>
        /// A Command to add multiple users at once
        /// </summary>
        public ICommand AddUsersCommand { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Raise the property change notify
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        public void Npc(string propertyName) => RaisePropertyChanged(propertyName);

        public void UsersSelectionChanged()
        {
            RaisePropertyChanged(nameof(HasSelectedUsers));
            RaisePropertyChanged(nameof(HasDisabledUsers));
            RaisePropertyChanged(nameof(HasEnabledUsers));
            RaisePropertyChanged(nameof(HasSingleSelectedUsers));
            RaisePropertyChanged(nameof(SelectedUsersCount));
        }

        #endregion
    }
}