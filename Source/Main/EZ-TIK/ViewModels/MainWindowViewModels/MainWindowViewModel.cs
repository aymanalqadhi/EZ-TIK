using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using EZ_TIK.Events;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        #region Private fields

        /// <summary>
        ///     The events manager injected by <see cref="Prism.Unity" />
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        ///     Deafult constructor
        /// </summary>
        /// <param name="eventAggregator"></param>
        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Title = "TEST";

            #region Subscribe Events

            // Subscribe to notification event
            _eventAggregator.GetEvent<Events.NotificationEvent>().Subscribe(e =>
            {
                Notifications.Enqueue(e.Message, e.ButtonText ?? "DISMISS", e.OnButtonCLick);

                if (e.SideNotification != null)
                {
                    var item = e.SideNotification;
                    item.CloseNotificationCommand = new DelegateCommand(() => SideNotifications.Remove(item));
                    SideNotifications.Insert(0, item);

                    int? count = SideNotifications.Count(n => n.IsNew);

                    _eventAggregator.GetEvent<UnseenNotificationsCountChangedEvent>().Publish(count);
                }

            });

            _eventAggregator.GetEvent<ToggleNotificationsTabEvent>().Subscribe(() => ToggleNotificationTabCommand.Execute(null));

            #endregion

            #region Init Commands

            ToggleNotificationTabCommand = new DelegateCommand(() =>
            {
                IsNotificationTabOpen ^= true;
                if (!IsNotificationTabOpen)
                {
                    SideNotifications.ForEach(n => n.IsNew = false);
                }
            });

            CloseNotificationsCommand = new DelegateCommand(() =>
            {
                IsNotificationTabOpen = false;
                SideNotifications.ForEach(n => n.IsNew = false);
                _eventAggregator.GetEvent<UnseenNotificationsCountChangedEvent>().Publish(null);
            });

            #endregion
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The title of the main window
        /// </summary>
        public string Title { get; set; } = "Test";

        /// <summary>
        /// Notifies list
        /// </summary>
        public SnackbarMessageQueue Notifications { get; set; } = new SnackbarMessageQueue();

        /// <summary>
        /// List of the side notification bar
        /// </summary>
        public ObservableCollection<SideNotificationViewModel> SideNotifications { get; set; } = new ObservableCollection<SideNotificationViewModel>();

        /// <summary>
        /// True if the notifications panel is open
        /// </summary>
        public bool IsNotificationTabOpen { get; set; }

        #endregion

        #region Commands

        public ICommand ToggleNotificationTabCommand { get; set; }

        public ICommand CloseNotificationsCommand { get; set; }

        #endregion
    }
}