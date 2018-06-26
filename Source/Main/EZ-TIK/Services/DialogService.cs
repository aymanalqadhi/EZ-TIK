using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using Prism.Events;

namespace EZ_TIK
{
    public class DialogService : IDialogService
    {
        #region Private members

        /// <summary>
        ///     <see cref="EventAggregator" /> injected by <see cref="Prism.Unity" />
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="eventAggregator">Dependency injection argument</param>
        public DialogService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion

        #region IDialogService Members

        /// <summary>
        ///     Shows a message with progress bar
        /// </summary>
        /// <param name="title">the title of the meesage</param>
        /// <param name="message">the body of the message</param>
        /// <param name="settings">Settings to pass to the dialog</param>
        /// <returns>await able task</returns>
        public async Task<ProgressDialogController> ShowProgressAsync(string title, string message, MetroDialogSettings settings = null) => 
            await (Application.Current.MainWindow as MetroWindow).ShowProgressAsync(title, message, settings: settings ?? My.MaterialDialogSettings);

        /// <summary>
        ///     Shows Message and get the user return
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="message">The body of the message</param>
        /// <param name="style">The buttons of the message</param>
        /// <param name="settings">Settings to pass to the dialog</param>
        /// <returns>awaitable task with MessageDialogResult</returns>
        public async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style, MetroDialogSettings settings) =>
                await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(title, message, style, settings ?? My.MaterialDialogSettings);

        /// <summary>
        ///     Shows Input dialog
        /// </summary>
        /// <param name="title">The title of the dialog</param>
        /// <param name="message">The message of the dialog</param>
        /// <param name="settings">Settings to pass to the dialog</param>
        /// <returns>The user input or null if canceled</returns>
        public async Task<object> ShowInputAsync(string title, string message, MetroDialogSettings settings = null) =>
                await (Application.Current.MainWindow as MetroWindow).ShowInputAsync(title, message, settings ?? My.MaterialDialogSettings);

        /// <summary>
        /// Shows a modal dialog
        /// </summary>
        /// <param name="viewModelType"></param>
        public async Task ShowDialog(Type viewModelType)
        {
            var viewType = ResolveViewType(viewModelType);
            if (viewType == null) return;

            await DialogHost.Show(Activator.CreateInstance(viewType));
        }

        /// <summary>
        /// Shows a modal dialog
        /// </summary>
        /// <param name="viewModelType"></param>
        public void ShowModal(Type viewModelType)
        {
            var viewType = ResolveViewType(viewModelType);
            if (viewType == null) return;

            (Activator.CreateInstance(viewType) as Window)?.ShowDialog();
        }

        #endregion

        private Type ResolveViewType(Type viewModelType)
        {
            var typeName = viewModelType.FullName;
            if (typeName == null) return null;

            if (typeName.Contains("ViewModel"))
            {
                typeName = typeName.Replace("ViewModel", "View");
            }

            return Type.GetType(typeName);
        }

    }
}