using System.Windows;

namespace EZ_TIK
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        protected override void OnStartup(StartupEventArgs e)
        {

            //new Window { Content = new Views.AddUserManagerUserView(), Height = 400, Width = 400 , WindowStartupLocation = WindowStartupLocation.CenterScreen }.Show();
            //return;
            base.OnStartup(e);
            Bootstrapper.Run();
        }
    }
}