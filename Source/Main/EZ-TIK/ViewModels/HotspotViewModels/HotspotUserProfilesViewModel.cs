using Prism.Mvvm;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using MaterialDesignThemes.Wpf;
using EZ_TIK.Models;
using MahApps.Metro.Controls.Dialogs;
using Prism.Events;
using EZ_TIK.Events;
using Microsoft.Practices.ObjectBuilder2;
using System;
using EZ_TIK.Views;
using EZ_TIK.ViewModels.HotspotViewModels;
using System.Threading.Tasks;

namespace EZ_TIK.ViewModels
{
    public class HotspotUserProfilesViewModel : BindableBase
    {
        #region Private fields

        /// <summary>
        /// The dialog service injected by the unity container
        /// </summary>
        private readonly IDialogService _dialogService;

        /// <summary>
        /// The Hotspot service injected by the unity container
        /// </summary>
        private readonly IHotspotClient _hotspotClient;

        /// <summary>
        /// The service that handles the events of the application 
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Contructors

        /// <summary>
        /// Default contructor
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="hotspotClinet"></param>
        public HotspotUserProfilesViewModel(IDialogService dialogService, IHotspotClient hotspotClinet, IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _hotspotClient = hotspotClinet;
            _eventAggregator = eventAggregator;

            Profiles = new ObservableCollection<HotspotUserProfileViewModel>();

            // Call Helper methods
            InitCommands();
            SubscribeEvents();

            // Refresh the profiles list
            RefreshCommand.Execute(null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A helper method to initilize the viewmodel commands
        /// </summary>
        private void InitCommands()
        {
            // Init the refresh command
            RefreshCommand = new DelegateCommand(async () => await DialogHost.Show(new Resources.ProgressDialog(), "RootDialog", async (s, e) =>
                {
                    // fetch profiles from server
                    var profiles = (await _hotspotClient.LoadAllProfilesAsync()).ToList().Select(u => new HotspotUserProfileViewModel(u) { SelectionChangedCommand = SharedItemSelectionChangedCommand, DeleteProfileCommand = SharedDeleteProfileCommand });

                    // Update user list
                    Profiles.Clear();
                    Profiles.AddRange(profiles);

                    e.Session.Close();

                    RaisePropertyChanged(nameof(ProfilesCount));
                    RaisePropertyChanged(nameof(SelectedCount));
                    RaisePropertyChanged(nameof(HasSelectedProfiles));

                    GC.Collect();

                }, null));

            // Init the deleting command
            RemoveSelectedProfilesCommand = new DelegateCommand(async () =>
            {
                var selected = Profiles.Where(p => p.IsSelected && p.Name != "default").ToList();
                if (selected.Count <= 0) return;

                var confirm = _dialogService.ShowMessageAsync("Confirm", $"Are sure that you want to delete the selected {selected.Count} selected profiles?", MessageDialogStyle.AffirmativeAndNegative);
                if (await confirm == MessageDialogResult.Negative) return;

                var dlg = await _dialogService.ShowProgressAsync("Deleting...", $"Deleting profile 1 of {selected.Count}...");
                dlg.SetCancelable(true);

                int done = 0;
                for (int i = 0; i < selected.Count; i++)
                {
                    if (dlg.IsCanceled) break;

                    dlg.SetMessage($"Deleting profile {i + 1} of {selected.Count}...");
                    dlg.SetProgress((double)i / selected.Count);

                    if (await _hotspotClient.RemoveProfileAsync(selected[i].UserProfileModel))
                    {
                        Profiles.Remove(selected[i]);
                        ++done;
                    }
                }

                var msg = $"Deleted {done} profiles successfully!";
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs(msg, null, new SideNotificationViewModel("Hotspot User Profiles", msg)));

                await dlg.CloseAsync();
                RefreshSelectionState();
                GC.Collect();
            });

            //Init the global delete command
            SharedDeleteProfileCommand = new DelegateCommand<HotspotUserProfileViewModel>(async vm =>
            {
                if (vm?.Name == "default")
                {
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs("Can't delete the default hotspot user profile!"));
                    return;
                }

                var confirm = _dialogService.ShowMessageAsync("Confirm", $"Are you sure that you want to delete profile `{vm.Name}` ?", MessageDialogStyle.AffirmativeAndNegative);
                if (await confirm == MessageDialogResult.Negative) return;

                var done = _hotspotClient.RemoveProfileAsync(vm.UserProfileModel);
                var msg = await done ? $"Profile `{vm.Name}` Deleted Successfully!" : $"Can't Delete the profile";

                _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs(msg, null, new SideNotificationViewModel("Hotspot User Profiles", msg)));
                if (await done) Profiles.Remove(vm);

                RefreshSelectionState();
                GC.Collect();
            });

            // Init the global selection changed command
            SharedItemSelectionChangedCommand = new DelegateCommand(RefreshSelectionState);

            // Init the selection commands
            SelectAllCommand = new DelegateCommand(() =>
            {
                Profiles.ForEach(p => { if (p.Name == "default") return; p.IsSelected = true; });
                RefreshSelectionState(); 
            });

            // Init the invert selection command
            InverseSelectionCommand = new DelegateCommand(() => { Profiles.ForEach(p => p.IsSelected ^= true); RefreshSelectionState(); });

            // Init the command which will show the adding profile dialog
            AddProfileCommand = new DelegateCommand(() => DialogHost.Show(new AddHotspotUserProfileView { DataContext = new AddHotspotUserProfileViewModel(_eventAggregator, null) }));
        }

        /// <summary>
        /// A helper method to subscribe to MVVM events
        /// </summary>
        private void SubscribeEvents()
        {
            // Subscribe to adding event
            _eventAggregator.GetEvent<AddHotspotUserProfileEvent>().Subscribe(async vm =>
            {
                // Check if the profile is already exists
                if(Profiles.Any(p => p.Name == vm.Name))
                {
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new NotificationEventArgs("This profile is already exists!"));
                    return;
                }

                // Set the commands
                vm.SelectionChangedCommand = SharedItemSelectionChangedCommand;
                vm.DeleteProfileCommand = SharedDeleteProfileCommand;

                // Adding local method
                Task<bool> Add() => _hotspotClient.AddProfileAsync(vm.UserProfileModel);

                // Executing the local method and get the result
                var done = await Add();
                var msg = done ? "Profile Add Successfully!" : "Unable To Add The Profile!";

                // Show a notification
                _eventAggregator.GetEvent<NotificationEvent>().Publish
                (
                    new NotificationEventArgs
                    (
                        msg, done ? "DISMISS" : "RETRY",
                        done ? null : new Action(async () => await Add()), 
                        new SideNotificationViewModel("Hotspot User Profiles", msg)
                    )
                );

                // Add the profile to the list
                if (done) Profiles.Add(vm);
            });
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The profiles list
        /// </summary>
        public ObservableCollection<HotspotUserProfileViewModel> Profiles { get; set; }

        /// <summary>
        /// The count of the profiles list items
        /// </summary>
        public int ProfilesCount => Profiles.Count;

        /// <summary>
        /// The count of the selected profiles list selected items
        /// </summary>
        public int SelectedCount => Profiles.Count(p => p.IsSelected && p.Name != "default");

        /// <summary>
        /// True if there are selected items in the profiles list
        /// </summary>
        public bool HasSelectedProfiles => SelectedCount > 0;

        #endregion

        #region Commands

        /// <summary>
        /// A Command to refresh the profiles list
        /// </summary>
        public ICommand RefreshCommand { get; set; }

        /// <summary>
        /// A Command to execute when an item got selected-unselected
        /// </summary>
        public ICommand SharedItemSelectionChangedCommand { get; set; }

        /// <summary>
        /// A Command to delete a certain profile
        /// </summary>
        public ICommand SharedDeleteProfileCommand { get; set; }

        /// <summary>
        /// A Command to delete selected profiles
        /// </summary>
        public ICommand RemoveSelectedProfilesCommand { get; set; }

        /// <summary>
        /// A Command to select all elements in the profiles list
        /// </summary>
        public ICommand SelectAllCommand { get; set; }

        /// <summary>
        /// A Command to invsert the current selection in the profiles list
        /// </summary>
        public ICommand InverseSelectionCommand { get; set; }
        
        /// <summary>
        /// A Command to show the view which used to add a hotspot user profile
        /// </summary>
        public ICommand AddProfileCommand { get; set; }

        #endregion

        #region Public Methods

        public void RefreshSelectionState()
        {
            RaisePropertyChanged(nameof(ProfilesCount));
            RaisePropertyChanged(nameof(SelectedCount));
            RaisePropertyChanged(nameof(HasSelectedProfiles));
        }

        #endregion
    }
}
