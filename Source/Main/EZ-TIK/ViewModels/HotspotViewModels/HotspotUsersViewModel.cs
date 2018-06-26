using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EZ_TIK.Events;
using EZ_TIK.Models;
using EZ_TIK.Parsers;
using EZ_TIK.Utils;
using EZ_TIK.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using tik4net.Objects;
using tik4net.Objects.Ip.Hotspot;
using ProgressDialog = EZ_TIK.Resources.ProgressDialog;

namespace EZ_TIK.ViewModels
{
    public class HotspotUsersViewModel : BindableBase
    {
        #region Private members

        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IHotspotClient _hotspotClient;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="eventAggregator"></param>
        /// <param name="hotspotClient"></param>
        public HotspotUsersViewModel(IDialogService dialogService, IEventAggregator eventAggregator, IHotspotClient hotspotClient)
        {
            // Init Private members
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _hotspotClient = hotspotClient;

            // Init public properties
            Users = new ObservableCollection<HotspotUserViewModel>();


            // Init Commands
            InitCommands();

            // Subscribe events
            SubcribeEvents();

            // Refresh the users list
            RefreshCommand.Execute(null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Subscribe to the child events
        /// </summary>
        private void SubcribeEvents()
        {
            // Subscribe to add event

            #region Add One Hotspot user

            _eventAggregator.GetEvent<AddHotspotUserEvent>().Subscribe(async userVm =>
            {
                var isUpdate = userVm.IsUpdate;

                // TODO:
                // Optimize this code

                var user = userVm.User;

                if (isUpdate && Users.Any(u => u.Username == user.Name && u.Username != userVm.OriginalName))
                {
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs("Can't use this username because it is already used!"));
                    return;
                }

                if (!isUpdate && Users.Any(u => u.Username == user.Name))
                {
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs("This user is already exists!"));
                    return;
                }

                /* Setting the EMAIL */

                // update the current email for the validity if the email is already exists

                var validity = !string.IsNullOrEmpty(userVm.LimitsViewModel.Validity) ? userVm.LimitsViewModel.Validity : "unlimited";
                var price = !string.IsNullOrEmpty(userVm.LimitsViewModel.Price) ? userVm.LimitsViewModel.Price.TrimStart('0') : "free";
                var status = isUpdate && !string.IsNullOrEmpty(user.Email) && Regex.IsMatch(user.Email.Trim(), @"^\w+?@\w+?\.\w+$") ? user.Email.Split('.')[1] : "np";

                // Setting the new email
                var newEmail = $"{validity}@{price}.{status}";
                user.Email = newEmail;

                // add the user and get the result
                var done = isUpdate ? await _hotspotClient.UpdateUserAsync(user) : await _hotspotClient.AddUserAsync(user);

                // Calculate the notifcation message
                string message;
                if (done && isUpdate) message = "User updated successfully!";
                else if (done) message = "User added successfully!";
                else if (isUpdate) message = "Failed in updating user!";
                else message = "Failed in adding user!";

                // Show notification about the proccess
                _eventAggregator.GetEvent<NotificationEvent>()
                        .Publish(new NotificationEventArgs(message, done ? "DISMISS" : "RETRY",
                            async () =>
                            {
                                if (done) return;

                                if (isUpdate) await _hotspotClient.UpdateUserAsync(user);
                                else await _hotspotClient.AddUserAsync(user);

                            }, new SideNotificationViewModel("Hotspot Users", message)
                       )
                   );

                // return if failed
                if (!done) return;

                // Update the edited user
                if (user.Id != null)
                {
                    var usr = Users.SingleOrDefault(u => u.UserModel.Id == user.Id);
                    if (usr == null) return;

                    usr.Validity = validity != "unlimited" && Regex.IsMatch(validity, @"^\d+$") ? int.Parse(validity) : -1;
                    usr.SetModel(user);
                }

                // Add the created user to th users list
                else Users.Add(new HotspotUserViewModel(user));
            });

            #endregion

            #region Add Multiple Hotspot Users

            _eventAggregator.GetEvent<AddMultipleHotspotUsersEvent>().Subscribe(async usersVm =>
            {
                var countObj = await _dialogService.ShowInputAsync("Add Hotspot Users", "Enter the count of the users you want to add");
                if (countObj == null) return;

                int count;
                if (!int.TryParse(countObj.ToString(), out count))
                {
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs("Invalid input!"));
                    return;
                }

                var dialog = await _dialogService.ShowProgressAsync("Adding users", $"Adding user - of {count}");
                dialog.SetCancelable(true);

                int[] mul = { 1024, 1048576, 1073741824 };

                long limitBytesOut = 0, limitBytesIn = 0;

                if (usersVm.ProfileAndLimitsViewModel.HasBandwidthLimits)
                {
                    limitBytesOut = long.Parse(usersVm.ProfileAndLimitsViewModel.DownloadAmount ?? "0") * mul[usersVm.ProfileAndLimitsViewModel.DownloadUnitType];
                    limitBytesIn = long.Parse(usersVm.ProfileAndLimitsViewModel.UploadAmount ?? "0") * mul[usersVm.ProfileAndLimitsViewModel.UploadUnitType];
                }

                var limitUptime = "00:00:00";
                if (usersVm.ProfileAndLimitsViewModel.HasTimeLimits)
                {
                    limitUptime = (new TimeSpan(int.Parse(usersVm.ProfileAndLimitsViewModel.TimeLimitDays ?? "0"), int.Parse(usersVm.ProfileAndLimitsViewModel.TimeLimitHours ?? "0"), int.Parse(usersVm.ProfileAndLimitsViewModel.TimeLimitMinutes ?? "0"), 0)).ToString().Replace(".", "d ");
                }

                var price = !string.IsNullOrEmpty(usersVm.ProfileAndLimitsViewModel.Price?.Trim()) ? usersVm.ProfileAndLimitsViewModel.Price : "free";
                var validity = !string.IsNullOrEmpty(usersVm.ProfileAndLimitsViewModel.Validity?.Trim()) ? usersVm.ProfileAndLimitsViewModel.Validity : "unlimited";
                var email = $"{validity}@{price}.np";

                var pass = 0;
                for (int i = 0, j = 0; i < count;)
                {
                    if (dialog.IsCanceled) break;


                    string name, password;

                    if (usersVm.UsernameAndPasswordViewModel.IsUsernameRandomlyGenerated)
                    {
                        var nameKind = usersVm.UsernameAndPasswordViewModel.IsUsernameLettersOnly ? RandomStringKind.CharactersOnly : usersVm.UsernameAndPasswordViewModel.IsUsernameNumbersOnly ? RandomStringKind.NumbersOnly : RandomStringKind.Mixed;
                        name = StringUtils.GetRandomString(nameKind, int.Parse(usersVm.UsernameAndPasswordViewModel.UsernameLength));
                    }
                    else
                    {
                        name = $"{usersVm.UsernameAndPasswordViewModel.UsernameStartCharacters}{int.Parse(usersVm.UsernameAndPasswordViewModel.UsernameStartNumber) + j++}";
                    }

                    if (Users.Any(u => u.Username == name)) continue;

                    dialog.SetMessage($"Adding user ( {i} : {name} ) of {count}");
                    dialog.SetProgress((double)i / count);


                    if (usersVm.UsernameAndPasswordViewModel.IsPasswordTheSameAsUsername) password = name;

                    else
                    {
                        var passKind = usersVm.UsernameAndPasswordViewModel.IsPasswordLettersOnly ? RandomStringKind.CharactersOnly : usersVm.UsernameAndPasswordViewModel.IsPasswordNumbersOnly ? RandomStringKind.NumbersOnly : RandomStringKind.Mixed;
                        password = StringUtils.GetRandomString(passKind, name.Length);
                    }


                    var user = new HotspotUser
                    {
                        Name = name,
                        Password = password,
                        Profile = "default",
                        LimitBytesOut = limitBytesOut,
                        LimitBytesIn = limitBytesIn,
                        LimitUptime = limitUptime,
                        Email = email
                    };

                    if (await _hotspotClient.AddUserAsync(user))
                    {
                        Users.Add(new HotspotUserViewModel(user));
                        pass++;
                    }

                    i++;
                }

                await dialog.CloseAsync();

                _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs($"{pass} Users added successfully!", null, new SideNotificationViewModel("Hotspot Users", $"{pass} Users added successfully!")));
                RaisePropertyChanged(nameof(UsersCount));

            });

            #endregion
        }

        /// <summary>
        /// Init commands
        /// </summary>
        private void InitCommands()
        {
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
                        var done = await _hotspotClient.RemoveUserAsync(Users.Single(u => u.IsSelected).UserModel);
                        if (done) Users.Remove(Users.SingleOrDefault(u => u.IsSelected));

                        // Inform the user
                        _eventAggregator.GetEvent<NotificationEvent>().Publish(
                            done
                                ? new NotificationEventArgs("Deleted User Successfully!", "UNDO", null, new SideNotificationViewModel("Hotspot Users", "Deleted User Successfully!"))
                                : new NotificationEventArgs("Failed in deleting user", "RETRY",
                                    () => deleteAction.Invoke(), new SideNotificationViewModel("Hotspot Users", "Deleted User Successfully!"))
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
                    if (!await _hotspotClient.RemoveUserAsync(selected[i])) continue;

                    // Update the UI
                    var item = Users.SingleOrDefault(u => u.Username == selected[i].Name);
                    if (item != null) Users.Remove(item);
                }

                // Close the progress dialog
                await loading.CloseAsync();

                // Show Notifcation
                _eventAggregator.GetEvent<NotificationEvent>()
                    .Publish(new NotificationEventArgs($"Deleted {i} items successfully!", null, new SideNotificationViewModel("Hotspot Users", $"Deleted {i + 1} items successfully!")));

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
                    if (await _hotspotClient.UpdateUserAsync(Users[selected[0]].UserModel))
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
                    if (!await _hotspotClient.UpdateUserAsync(Users[selected[i]].UserModel))
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

            #region Refresh Command

            // Init Refresh Command 
            RefreshCommand = new DelegateCommand<DataGrid>(async dg =>
            {
                await DialogHost.Show(new ProgressDialog(), "RootDialog", async (s, e) =>
                {
                    // fetch users from server
                    var users = (await _hotspotClient.LoadAllUsersAsync()).ToList().Select(u => new HotspotUserViewModel(u));

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

            #region Search Command

            // A Command to find a user from the search box
            SearchCommand = new DelegateCommand<DataGrid>(async dg =>
            {
                if (string.IsNullOrEmpty(SearchText?.Trim())) return;

                await Task.Run(() =>
                {
                    // found items list
                    var found = new List<HotspotUserViewModel>();

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

            #region AddUserCommand

            AddUserCommand = new DelegateCommand(async () => await DialogHost.Show(new AddHotspotUserView()));

            #endregion

            #region Add Users Command

            AddUsersCommand = new DelegateCommand(async () => await DialogHost.Show(new AddMultipleHotspotUsersView()));

            #endregion

            #region EditUserCommand

            EditUserCommand = new DelegateCommand(async () =>
            {
                var user = Users.SingleOrDefault(u => u.IsSelected);
                if (user == null) return;

                var addDialogViewModel = new AddHotspotUserViewModel(_eventAggregator, _hotspotClient, new HotspotUserViewModel(user.UserModel.CloneEntity()));

                await DialogHost.Show(new AddHotspotUserView { DataContext = addDialogViewModel });
            });

            #endregion

            #region Reset Command

            ResetCommand = new DelegateCommand(async () =>
            {
                // return if there is no selected users
                if (!HasSelectedUsers) return;

                // get selected users
                var selected = Users.Where(u => u.IsSelected).Select(u => Users.IndexOf(u)).ToList();

                // Do Single Action
                if (selected.Count == 1)
                {
                    // Get a clone of the selected user model
                    var user = Users[selected[0]];

                    // Clone properties
                    ByteSize lbi = user.LimitBytesIn, lbo = user.LimitBytesOut, lbt = user.LimitBytesTotal;
                    string profile = user.Profile, lut = user.LimitUptime, ip = user.IpAddress, mac = user.MacAddress;

                    // reset properties
                    user.LimitBytesOut = user.LimitBytesIn = user.LimitBytesTotal = ByteSize.MinValue;
                    user.LimitUptime = "00:00:00";
                    user.Profile = "default";
                    user.MacAddress = "00:00:00:00:00:00";
                    user.IpAddress = "0.0.0.0";

                    string message;

                    if (await _hotspotClient.UpdateUserAsync(user.UserModel))
                    {
                        message = $"The user \"{user.Username}\" has been reset successfully!";
                    }
                    else
                    {
                        message = $"Can't reset user \"{user.Username}\" !";

                        // Set old values back
                        user.IpAddress = ip;
                        user.MacAddress = mac;
                        user.LimitBytesOut = lbo;
                        user.LimitBytesIn = lbi;
                        user.LimitBytesTotal = lbt;
                        user.LimitUptime = lut;
                        user.Profile = profile;
                    }

                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs(message, null, new SideNotificationViewModel("Hotspot Users", message)));

                    // Update the UI
                    UsersSelectionChanged();

                    return;
                }

                /* DO Multiple reset operation */

                // Init loadng dialog
                var loading = await _dialogService.ShowProgressAsync("Reseting Users", "Reseting user - ...");
                loading.SetCancelable(true);

                var fail = 0;
                for (var i = 0; i < selected.Count; i++)
                {
                    if (loading.IsCanceled) break;

                    // Get a clone of the selected user model
                    var user = Users[selected[i]];

                    // Update the loading dialog
                    loading.SetProgress((double)i / selected.Count);
                    loading.SetMessage($"Reseting user \"{user.Username}\" ...");

                    // Clone properties
                    ByteSize lbi = user.LimitBytesIn, lbo = user.LimitBytesOut, lbt = user.LimitBytesTotal;
                    string profile = user.Profile, lut = user.LimitUptime, ip = user.IpAddress, mac = user.MacAddress;

                    // reset properties
                    user.LimitBytesOut = user.LimitBytesIn = user.LimitBytesTotal = ByteSize.MinValue;
                    user.LimitUptime = "00:00:00";
                    user.Profile = "default";
                    user.MacAddress = "00:00:00:00:00:00";
                    user.IpAddress = "0.0.0.0";

                    if (!await _hotspotClient.UpdateUserAsync(user.UserModel))
                    {
                        fail++;

                        // Set old values back
                        user.IpAddress = ip;
                        user.MacAddress = mac;
                        user.LimitBytesOut = lbo;
                        user.LimitBytesIn = lbi;
                        user.LimitBytesTotal = lbt;
                        user.LimitUptime = lut;
                        user.Profile = profile;
                    }
                }

                // Close the loading dialog and show a report about the operation
                await loading.CloseAsync();
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs($"{selected.Count - fail} User have been reset!", null, new SideNotificationViewModel("Hotspot Users", $"{selected.Count - fail} User have been reset!")));
            });

            #endregion
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Users List fetched from the router
        /// </summary>
        public ObservableCollection<HotspotUserViewModel> Users { get; set; }

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
        /// A Command to remove seleced users
        /// </summary>
        public ICommand RemoveCommand { get; set; }

        /// <summary>
        /// A Command to toggle the Disabled property of the selected users
        /// </summary>
        public ICommand ToggleCommand { get; set; }

        /// <summary>
        /// A Command to reset the main properties of selected user
        /// </summary>
        public ICommand ResetCommand { get; set; }

        /// <summary>
        /// A Command to refresh the users list
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        /// A Command to find user
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// A Command to show the AddHotspotUser dialog
        /// </summary>
        public ICommand AddUserCommand { get; set; }

        /// <summary>
        /// A Command to show AddMultipleHotspotUsers dialog
        /// </summary>
        public ICommand AddUsersCommand { get; set; }

        /// <summary>
        /// A Command to edit the selected user
        /// </summary>
        public ICommand EditUserCommand { get; set; }
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