using System;
using System.Windows.Input;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class SideNotificationViewModel : BindableBase
    {
        public SideNotificationViewModel(string notificationTitle, object notificationContent)
        {
            NotificationTitle = notificationTitle;
            NotificationContent = notificationContent;

            IsNew = true;
            NotificationTime = DateTime.Now;
        }

        public string NotificationTitle { get; set; }
        public object NotificationContent { get; set; }
        public DateTime NotificationTime { get; set; }
        public char FirstCharOfTheTitle => !string.IsNullOrEmpty(NotificationTitle) ? NotificationTitle[0] : (char)(new Random()).Next(65, 89);
        public bool IsNew { get; set; }

        public ICommand CloseNotificationCommand { get; set; }
    }
}
