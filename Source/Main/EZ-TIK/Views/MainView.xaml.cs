using System.Windows.Controls;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Prism.Regions;

namespace EZ_TIK.Views
{
    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        
        public MainView(IRegionManager regionManager)
        {
            InitializeComponent();
            Loaded += (s, e) => regionManager.RequestNavigate(My.Regions[Region.MainRegion], My.Views[View.UserManagerProfilesView]);
        }


    }
}