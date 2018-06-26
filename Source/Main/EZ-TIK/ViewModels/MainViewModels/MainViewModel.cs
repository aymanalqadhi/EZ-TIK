using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EZ_TIK.Events;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace EZ_TIK.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region Constructors

        /// <summary>
        ///     Deafult constructor
        /// </summary>
        /// <param name="regionManager"></param>
        public MainViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            #region Init Commands

            // Init Navigation Command
            NavigationCommand = new DelegateCommand<string>(uri =>
            {
                RaisePropertyChanged(nameof(MainContentAnimation));

                regionManager.RequestNavigate(My.Regions[Region.MainRegion], uri);
                ISideMenuItem item = null;

                foreach (var category in SideMenuListItems)
                {
                    if(!(category is SideMenuListCategoryViewModel)) continue;

                    foreach (var sideItem in ((SideMenuListCategoryViewModel)category).Items)
                    {
                        if (sideItem.CommandParameter == uri)
                        {
                            item = sideItem;
                            break;
                        }
                    }
                }

                if (item == null) return;
                CurrentViewTitle = item.Header.ToString();
            });

            // ToggleNotificationsTab
            ToggleNotificationsTabCommand = new DelegateCommand(() => eventAggregator.GetEvent<ToggleNotificationsTabEvent>().Publish());

            #endregion

            regionManager.RequestNavigate(My.Regions[Region.MainRegion], My.Views[View.UserManagerProfilesView]);

            eventAggregator.GetEvent<UnseenNotificationsCountChangedEvent>().Subscribe(c => UnseenNotificationsCount = c);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The title of the current view
        /// </summary>
        public string CurrentViewTitle { get; set; }

        /// <summary>
        ///     The side menu items
        /// </summary>
        public ICollection<ISideMenuItem> SideMenuListItems { get; set; } =
            new ObservableCollection<ISideMenuItem>
            {
                new SideMenuListCategoryViewModel
                {
                    Header = "Hotspot",
                    Items = new ObservableCollection<SideMenuListItemViewModel>
                    {
                        new SideMenuListItemViewModel
                        {
                            Header = "Hotspot Users",
                            CommandParameter = My.Views[View.HotspotUsersView]
                        },
                        new SideMenuListItemViewModel
                        {
                            Header = "Hotspot User Profiles",
                            CommandParameter = My.Views[View.HotspotUserProfilesView]
                        }
                    }
                },
                 new SideMenuListCategoryViewModel
                {
                    Header = "User Manager",
                    Items = new ObservableCollection<SideMenuListItemViewModel>
                    {
                        new SideMenuListItemViewModel
                        {
                            Header = "User-Manager Users",
                            CommandParameter = My.Views[View.UserManagerUsersView]
                        },
                        new SideMenuListItemViewModel
                        {
                            Header = "User-Manager Profiles",
                            CommandParameter = My.Views[View.UserManagerProfilesView]
                        }
                    }
                }
            };

        public int SelectedSideMenuIndex { get; set; }

        public int? UnseenNotificationsCount { get; set; }

        public TransitionType MainContentAnimation => (TransitionType)My.Rnd.Next(0, 7);


        #endregion

        #region Commands

        /// <summary>
        ///     The navigation command
        /// </summary>
        public ICommand NavigationCommand { get; set; }

        /// <summary>
        /// A Command to toggle the notifications tab
        /// </summary>
        public ICommand ToggleNotificationsTabCommand { get; set; }

        #endregion
    }

    public interface ISideMenuItem
    {
        object Header { get; set; }
    }
}
