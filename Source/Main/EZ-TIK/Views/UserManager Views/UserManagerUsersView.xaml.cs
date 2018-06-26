using System.Windows.Controls;
using EZ_TIK.ViewModels;
using Microsoft.Practices.ObjectBuilder2;

namespace EZ_TIK.Views
{
    /// <summary>
    /// Interaction logic for UserManagerUsersView.xaml
    /// </summary>
    public partial class UserManagerUsersView : UserControl
    {
        public UserManagerUsersView()
        {
            InitializeComponent();
        }

        private void UsersList_SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            e.AddedCells.ForEach(i =>
            {
                if (i.Item is UserManagerUserViewModel item) item.IsSelected = true;
            });

            e.RemovedCells.ForEach(i =>
            {
                if (i.Item is UserManagerUserViewModel item) item.IsSelected = false;
            });

            // Update the selection label
            (DataContext as dynamic).UsersSelectionChanged();
        }
    }
}
