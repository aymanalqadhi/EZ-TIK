using System.Windows;
using System.Windows.Controls;

namespace EZ_TIK.Views
{
    /// <summary>
    /// Interaction logic for AddMultipleHotspotUsersUsernameAndPasswordView.xaml
    /// </summary>
    public partial class AddMultipleUsersUsernameAndPasswordView : UserControl
    {
        public AddMultipleUsersUsernameAndPasswordView()
        {
            InitializeComponent();
        }

        private void IsPasswordTheSameAsUsername_Changed(object sender, RoutedEventArgs e) => (DataContext as dynamic).IsPasswordTheSameAsUsername = ((CheckBox) sender).IsChecked.GetValueOrDefault();
    }
}
