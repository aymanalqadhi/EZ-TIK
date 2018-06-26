using System;
using EZ_TIK.ViewModels;
using Prism.Events;

namespace EZ_TIK.Events
{
    public class NotificationEvent : PubSubEvent<NotificationEventArgs> { }

    public class NotificationEventArgs
    {
        #region Public Properties

        /// <summary>
        /// The message to show in the notification
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The text to show on the button
        /// </summary>
        public string ButtonText { get; set; }

        /// <summary>
        /// An Event to raise when the button is clicked
        /// </summary>
        public Action OnButtonCLick { get; set; }

        public SideNotificationViewModel SideNotification { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor to implement a full nofitiation
        /// </summary>
        /// <param name="message">The message to show on the notification</param>
        /// <param name="buttonText">The Text to show on the notification button</param>
        /// <param name="onButtonCLick">The method to execute on notification button click</param>
        /// <param name="sideNotification"></param>
        public NotificationEventArgs(string message, string buttonText, Action onButtonCLick, SideNotificationViewModel sideNotification = null)
        {
            Message = message;
            ButtonText = buttonText;
            OnButtonCLick = onButtonCLick;
            SideNotification = sideNotification;
        }

        /// <summary>
        /// Constructor to implement a nofitiation with button text UNDO
        /// </summary>
        /// <param name="message">The message to show on the notification</param>
        /// <param name="onButtonCLick">The method to execute on notification button click</param>
        /// <param name="sideNotification"></param>
        public NotificationEventArgs(string message, Action onButtonCLick, SideNotificationViewModel sideNotification = null) : this(message, null, onButtonCLick, sideNotification) { }

        /// <summary>
        /// Constructor to implement a standard nofitiation
        /// </summary>
        /// <param name="message">The message to show on the notification</param>
        public NotificationEventArgs(string message) : this(message, null, null) { } 

        #endregion
    }
}
