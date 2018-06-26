using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace EZ_TIK.ViewModels
{
    public class SideMenuListCategoryViewModel : BindableBase, ISideMenuItem
    {
        /// <summary>
        /// True if the drop down is open
        /// </summary>
        public bool IsOpened { get; set; }

        /// <summary>
        /// The Header of the category
        /// </summary>
        public object Header { get; set; }

        /// <summary>
        /// The items of the category
        /// </summary>
        public ObservableCollection<SideMenuListItemViewModel> Items { get; set; }
    }
}
