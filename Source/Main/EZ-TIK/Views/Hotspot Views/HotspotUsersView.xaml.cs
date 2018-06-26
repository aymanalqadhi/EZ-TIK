using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using EZ_TIK.ViewModels;
using Microsoft.Practices.ObjectBuilder2;

namespace EZ_TIK.Views
{
    /// <summary>
    ///     Interaction logic for HotspotUsers.xaml
    /// </summary>
    public partial class HotspotUsersView : UserControl
    {
        public HotspotUsersView()
        {
            InitializeComponent();
        }

        private void UsersList_SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            e.AddedCells.ForEach(i =>
            {
                var item = i.Item as HotspotUserViewModel;
                if (item != null) item.IsSelected = true;
            });

            e.RemovedCells.ForEach(i =>
            {
                var item = i.Item as HotspotUserViewModel;
                if (item != null) item.IsSelected = false;
            });

            // Update the selection label
            (DataContext as dynamic).UsersSelectionChanged();
        }

    }
}