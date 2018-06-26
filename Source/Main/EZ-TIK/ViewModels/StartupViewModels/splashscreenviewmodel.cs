using System;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows;

namespace EZ_TIK.ViewModels
{
    public class SplashScreenViewModel : BindableBase
    {
        public SplashScreenViewModel(IRegionManager regionManager)
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                await Application.Current.Dispatcher.BeginInvoke(new Action(() => regionManager.RequestNavigate(My.Regions[Region.MainContent], My.Views[View.StartupView])));
            });
        }

        public string Title { get; set; } = "EZ-TIK";
    }
}