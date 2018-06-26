using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Prism.Mvvm;

namespace EZ_TIK
{
    public class ValidatedBindableBase : BindableBase, INotifyDataErrorInfo
    {
        #region Private Fields

        /// <summary>
        ///     Errors List
        /// </summary>
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Checks if the ViewModel has errors
        /// </summary>
        public bool HasErrors => _errors.Count > 0;

        #endregion

        #region Event Handlers

        /// <summary>
        ///     Event to fire when error is added or removed
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Get errors list from the <see cref="_errors" /> Dictionary
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
            => _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;

        #endregion

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var setted = base.SetProperty(ref storage, value, propertyName);
            ValidateProperty(propertyName, value);
            return setted;
        }

        protected virtual void ValidateProperty<T>(string propertyName, T value)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };

            Validator.TryValidateObject(this, context, results);

            if (results.Any())
                _errors[propertyName] = results.Select(c => c.ErrorMessage).ToList();
            else
                _errors.Remove(propertyName);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}