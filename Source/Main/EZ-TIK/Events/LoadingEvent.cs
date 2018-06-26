using System;
using Prism.Events;

namespace EZ_TIK.Events
{
    /// <summary>
    ///     An events that opens a progress dialog
    /// </summary>
    public class ShowProgressEvent : PubSubEvent<ShowLoadingEventArgs>
    {
    }

    /// <summary>
    ///     An events that hides a progress dialog
    /// </summary>
    public class HideProgressEvent : PubSubEvent<ShowLoadingEventArgs>
    {
    }

    /// <summary>
    ///     Arguemts to pass using the <see cref="ShowProgressEvent" /> and <see cref="HideProgressEvent" /> events
    /// </summary>
    public class ShowLoadingEventArgs : EventArgs, IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Deafult constructor
        /// </summary>
        /// <param name="title">The title of the progress dialog</param>
        /// <param name="message">The message of the progress dialog</param>
        /// <param name="isIndeterminate">THe dialog progress bar type</param>
        public ShowLoadingEventArgs(string title, string message, bool isIndeterminate = false)
        {
            Title = title;
            Message = message;
            IsIndeterminate = isIndeterminate;
        }

        #endregion

        #region IDisposable Memebers

        /// <summary>
        ///     De-Attach event handlers
        /// </summary>
        public void Dispose()
        {
            ProgressChanged = null;
            TitleChanged = null;
            MessageChanged = null;
            IsIndeterminateChanged = null;
        }

        #endregion

        #region Private Members

        /// <summary>
        ///     The progress of the loading dialog
        /// </summary>
        private double _progress;

        /// <summary>
        ///     The title of the loading dialog
        /// </summary>
        private string _title;

        /// <summary>
        ///     The message of the loading dialog
        /// </summary>
        private string _message;

        /// <summary>
        ///     The propgess of the loading dialog
        /// </summary>
        private bool _isInderteminate;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The loading dialog title
        /// </summary>
        public string Title
        {
            get => _title; set
            {
                if (_title == value) return;

                _title = value;
                OnTitleChanged(this);
            }
        }

        /// <summary>
        ///     The loading dialog message
        /// </summary>
        public string Message
        {
            get => _message; set
            {
                if (_message == value) return;

                _message = value;
                OnMessageChanged(this);
            }
        }

        /// <summary>
        ///     The value propgress bar is indetermine
        /// </summary>
        public bool IsIndeterminate
        {
            get => _isInderteminate; set
            {
                if (_isInderteminate == value) return;

                _isInderteminate = value;
                OnIsIndeterminateChanged(this);
            }
        }

        /// <summary>
        ///     The progress of the loading dialog
        /// </summary>
        public double Progress
        {
            get => _progress; set
            {
                if (_progress == value) return;

                _progress = value;
                OnProgressChanged(this);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        ///     Event handler that fires on the <see cref="Progress" /> property changes
        /// </summary>
        public event EventHandler<ShowLoadingEventArgs> ProgressChanged;


        /// <summary>
        ///     Event handler that fires on the <see cref="Title" /> property changes
        /// </summary>
        public event EventHandler<ShowLoadingEventArgs> TitleChanged;

        /// <summary>
        ///     Event handler that fires on the <see cref="Message" /> property changes
        /// </summary>
        public event EventHandler<ShowLoadingEventArgs> MessageChanged;

        /// <summary>
        ///     Event handler that fires on the <see cref="IsIndeterminate" /> property changes
        /// </summary>
        public event EventHandler<ShowLoadingEventArgs> IsIndeterminateChanged;

        #endregion

        #region Event Handlers

        /// <summary>
        ///     <see cref="ProgressChanged" /> Event handler
        /// </summary>
        /// <param name="e">Event arugments</param>
        protected virtual void OnProgressChanged(ShowLoadingEventArgs e) => ProgressChanged?.Invoke(this, e);

        /// <summary>
        ///     <see cref="TitleChanged" /> Event handler
        /// </summary>
        /// <param name="e">Event arugments</param>
        protected virtual void OnTitleChanged(ShowLoadingEventArgs e) => TitleChanged?.Invoke(this, e);

        /// <summary>
        ///     <see cref="MessageChanged" /> Event handler
        /// </summary>
        /// <param name="e">Event arugments</param>
        protected virtual void OnMessageChanged(ShowLoadingEventArgs e) => MessageChanged?.Invoke(this, e);

        /// <summary>
        ///     <see cref="IsIndeterminateChanged" /> Event handler
        /// </summary>
        /// <param name="e">Event arugments</param>
        protected virtual void OnIsIndeterminateChanged(ShowLoadingEventArgs e)
            => IsIndeterminateChanged?.Invoke(this, e);

        #endregion
    }
}