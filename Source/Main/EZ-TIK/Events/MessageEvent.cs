using System;
using Prism.Events;

namespace EZ_TIK.Events
{
    public class MessageEvent : PubSubEvent<MessageEventArgs>
    {
    }

    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        ///     Deafult constructor
        /// </summary>
        /// <param name="title">The title of the message</param>
        /// <param name="message">The body of the message</param>
        public MessageEventArgs(string title, string message)
        {
            Title = title;
            Message = message;
        }

        #region Public Properties

        /// <summary>
        ///     Gets or sets the title of the message
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the body of the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Gets the result of the message
        /// </summary>
        public bool DialogResult { get; private set; }

        #endregion
    }
}