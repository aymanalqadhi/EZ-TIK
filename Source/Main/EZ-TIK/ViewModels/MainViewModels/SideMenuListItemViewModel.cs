using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class SideMenuListItemViewModel : BindableBase, ISideMenuItem
    {
        #region Private Members

        private string _commandParam;
        private object _header;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the header text of the item
        /// </summary>
        public object Header
        {
            get => _header; set
            {
                if (value == _header) return;
                SetProperty(ref _header, value);
            }
        }

        /// <summary>
        ///     Gets or sets the command parameter used to navigate
        /// </summary>
        public string CommandParameter
        {
            get => _commandParam; set
            {
                if (value == _commandParam) return;
                SetProperty(ref _commandParam, value);
            }
        }

        #endregion
    }
}