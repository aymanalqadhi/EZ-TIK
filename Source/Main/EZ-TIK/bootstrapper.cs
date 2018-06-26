using System.ComponentModel;
using System.Linq;
using System.Windows;
using EZ_TIK.Models;
using EZ_TIK.Views;
using Microsoft.Practices.Unity;
using Prism.Unity;
using tik4net;
using tik4net.Objects;
using tik4net.Objects.Ip.Hotspot;

namespace EZ_TIK
{
    public class Bootstrapper : UnityBootstrapper
    {
        /// <summary>
        ///     Creates the main shell
        /// </summary>
        /// <returns>
        ///     <see cref="DependencyObject" />
        /// </returns>
        protected override DependencyObject CreateShell() => Container.Resolve<MainWindow>();

        /// <summary>
        ///     Initializes the created shell
        /// </summary>
        protected override void InitializeShell() => Application.Current.MainWindow.Show();

        /// <summary>
        ///     Configures the created shell
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterTypeForNavigation<SplashScreenView>(My.Views[View.SplashScreenView]);
            Container.RegisterTypeForNavigation<StartupView>(My.Views[View.StartupView]);
            Container.RegisterTypeForNavigation<MainView>(My.Views[View.MainView]);

            #region MainViews

            Container.RegisterTypeForNavigation<HomeView>(My.Views[View.HomeView]);
            Container.RegisterTypeForNavigation<HotspotUsersView>(My.Views[View.HotspotUsersView]);
            Container.RegisterTypeForNavigation<HotspotUserProfilesView>(My.Views[View.HotspotUserProfilesView]);
            Container.RegisterTypeForNavigation<UserManagerUsersView>(My.Views[View.UserManagerUsersView]);
            Container.RegisterTypeForNavigation<UserManagerProfilesView>(My.Views[View.UserManagerProfilesView]);

            #endregion

            Container.RegisterType<IDialogService, DialogService>();

            var conn = ConnectionFactory.OpenConnection(TikConnectionType.Api, "5.5.5.6", 8728, "admin", "");
            Container.RegisterInstance(typeof(ITikConnection), conn);

            Container.RegisterType<IHotspotClient, HotspotClient>();
            Container.RegisterType<IUserManagerClient, UserManagerClient>();
        }

    }
}