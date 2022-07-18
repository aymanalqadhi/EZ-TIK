using Prism.Regions;
using MahApps.Metro.Controls;

namespace EZ_TIK.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>                                                   s
    public partial class MainWindow : MetroWindow
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();

            // Navigate to startup view on startup
            Loaded += (s, e) => regionManager.RequestNavigate(My.Regions[Region.MainContent], My.Views[View.SplashScreenView]);
        }

    }
}