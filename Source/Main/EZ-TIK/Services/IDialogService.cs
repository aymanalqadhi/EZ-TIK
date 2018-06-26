using System;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;

namespace EZ_TIK
{
    public interface IDialogService
    {
        Task<ProgressDialogController> ShowProgressAsync(string title, string message, MetroDialogSettings settings = null);
        Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style, MetroDialogSettings settings = null);
        Task<object> ShowInputAsync(string title, string message, MetroDialogSettings settings = null);
        Task ShowDialog(Type viewModelType);
        void ShowModal(Type viewModelType);
    }
}