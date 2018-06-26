using System.Windows;

namespace EZ_TIK.Views
{
    /// <summary>
    ///     Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Update the password dynamicaly=ly
        /// </summary>
        /// <param name="sender">The password box</param>
        /// <param name="e">Event args</param>
        private void Password_OnPasswordChanged(object sender, RoutedEventArgs e) => (DataContext as dynamic).Password = Password.Password;
    }
}