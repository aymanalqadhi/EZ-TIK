using EZ_TIK.ViewModels;
using Microsoft.Practices.ObjectBuilder2;
using System.Windows.Controls;

namespace EZ_TIK.Views
{
    /// <summary>
    /// Interaction logic for UserManagerProfilesView.xaml
    /// </summary>
    public partial class UserManagerProfilesView : UserControl
    {
        public UserManagerProfilesView()
        {
            InitializeComponent();
        }

        private void ProfilesList_SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            e.AddedCells.ForEach(i =>
            {
                if (i.Item is UserManagerProfileViewModel item) item.IsSelected = true;
            });

            e.RemovedCells.ForEach(i =>
            {
                if (i.Item is UserManagerProfileViewModel item) item.IsSelected = false;
            });

            // Update the selection label
            (DataContext as dynamic).RefreshItemsState();
        }
    }
}
