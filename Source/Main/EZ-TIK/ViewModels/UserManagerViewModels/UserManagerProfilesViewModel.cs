using EZ_TIK.Events;
using EZ_TIK.Models;
using EZ_TIK.Resources;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EZ_TIK.ViewModels
{
    public class UserManagerProfilesViewModel : BindableBase
    {
        #region Private Fields

        /// <summary>
        /// A Service of managing dialogs
        /// </summary>
        private IDialogService _dialogService;

        /// <summary>
        /// A Service to manage the user manager
        /// </summary>
        private IUserManagerClient _userManagerClient;

        /// <summary>
        /// A Service to manage MVVM events
        /// </summary>
        private IEventAggregator _eventAggregator;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of the profiles
        /// </summary>
        public ObservableCollection<UserManagerProfileViewModel> Profiles { get; set; }

        /// <summary>
        /// Gets the count of the profiles list
        /// </summary>
        public int ItemsCount => Profiles?.Count ?? 0;

        /// <summary>
        /// Gets the count of selected items only
        /// </summary>
        public int SelectedCount => Profiles?.Count(p => p.IsSelected) ?? 0;

        /// <summary>
        /// True if the selected count is greater than 0
        /// </summary>
        public bool HasSelected => SelectedCount > 0;

        /// <summary>
        /// True if the selected count is 1
        /// </summary>
        public bool HasSingleSelected => SelectedCount == 1;

        /// <summary>
        /// Get true if all items in the profiles list are selected
        /// </summary>
        public bool IsAllSelected
        {
            get => Profiles.All(p => p.IsSelected);
            set => Profiles.AsParallel().ForAll(p => p.IsSelected = value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// <paramref name="dialogService">The Dialog service injected by unity. </paramref>
        /// </summary>
        public UserManagerProfilesViewModel(IDialogService dialogService, IUserManagerClient userManagerClient, IEventAggregator eventAggregator)
        {
            // Init the private fields
            _dialogService = dialogService;
            _userManagerClient = userManagerClient;
            _eventAggregator = eventAggregator;

            // Init Public Properties
            Profiles = new ObservableCollection<UserManagerProfileViewModel>();

            // Call helper memthods
            InitCommands();

            // Refresh the list for the first time
            RefreshCommand.Execute(null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A Helper method to initilize the viewmodel commands
        /// </summary>
        private void InitCommands()
        {
            // Init the refresh method
            RefreshCommand = new DelegateCommand(() =>
            {
                // Show a progress dialog
                DialogHost.Show(new ProgressDialog(), 
                    openedEventHandler: async (s, e) =>
                    {
                        // Fetch the profiles list
                        var profiles = (await _userManagerClient.LoadAllProfilesAsync()).ToList().Select(p => new UserManagerProfileViewModel(p) { SelectionChangeCommand = SharedSelectionChangeCommand });

                        // Update the display list
                        Profiles.Clear();
                        Profiles.AddRange(profiles);

                        // Reflect changes
                        RefreshItemsState();

                        // Close the dialog
                        e.Session.Close();
                    }
                );
            });

            // Init Shared Selection Command
            SharedSelectionChangeCommand = new DelegateCommand(() => RefreshItemsState());

            // Init the deleting command
            DeleteSelectedProfilesCommand = new DelegateCommand(async () =>
            {
                var prof = Profiles.Where(p => p.IsSelected).ToList();
                if (prof == null) return;

                var multi = prof.Count > 1;

                var conf = await _dialogService.ShowMessageAsync("Confirm deletion", "Are you really sure that you want to delete selected pofiles?", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative);
                if (conf != MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative) return;

                async Task deleteProfile()
                {
                    if(multi)
                    {
                        var dlg = await _dialogService.ShowProgressAsync("Deleting profiles", $"Deleting profile 1 of {prof.Count} ...");
                        var done = 0;

                        for (int i = 0; i < prof.Count; ++i)
                        {
                            if (dlg.IsCanceled) break;

                            dlg.SetMessage($"Deleting profile {i + 1} from {prof.Count} ...");
                            dlg.SetProgress(i / (double)prof.Count);

                            if (await _userManagerClient.RemoveProfileAsync(prof[i].ProfileModel)) ++done;
                        }

                        var msg = $"Deleted {done} profiles successfully!";
                        _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs(msg, null, new SideNotificationViewModel("UserManager Profiles", msg)));
                    }
                    else
                    {
                        if (await _userManagerClient.RemoveProfileAsync(prof[0].ProfileModel))
                        {
                            _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs("Profile deleted successfully!", null, new SideNotificationViewModel("UserManager Profiles", "Profile deleted successfully!")));
                            Profiles.Remove(prof[0]);
                        }
                        else
                            _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs("Couldn't delete profile", "RETRY", async () => await deleteProfile()));
                    }
                }

                await deleteProfile();

            }).ObservesProperty(() => HasSingleSelected);
        }

        #endregion

        #region Commands

        /// <summary>
        /// A Command to refresh the profiles list
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        /// A Shared command to invoke on selection changes
        /// </summary>
        public ICommand SharedSelectionChangeCommand { get; set; }

        /// <summary>
        /// A Command to delete the selected profiles
        /// </summary>
        public ICommand DeleteSelectedProfilesCommand { get; set; }

        /// <summary>
        /// Edit the signle selected profile
        /// </summary>
        public ICommand EditSelectedProfileCommand { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// A Method to reflect the changes on the view
        /// </summary>
        public void RefreshItemsState()
        {
            RaisePropertyChanged(nameof(ItemsCount));
            RaisePropertyChanged(nameof(SelectedCount));
            RaisePropertyChanged(nameof(HasSelected));
            RaisePropertyChanged(nameof(HasSingleSelected));
            RaisePropertyChanged(nameof(IsAllSelected));
        }

        #endregion
    }
}
