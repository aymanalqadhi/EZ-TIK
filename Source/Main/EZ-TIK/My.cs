using System;
using System.Collections.Generic;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace EZ_TIK
{
    public enum View
    {
        SplashScreenView,
        MainView,
        HotspotUsersView,
        HotspotUserProfilesView,
        UserManagerUsersView,
        UserManagerProfilesView,
        StartupView,
        HomeView
    }

    public enum Region
    {
        MainContent = 0,
        MainRegion = 1
    }

    public static class My
    {
        public static Dictionary<View, string> Views;
        public static Dictionary<Region, string> Regions;

        public static MetroDialogSettings MaterialDialogSettings = new MetroDialogSettings
        {
            SuppressDefaultResources = true,
            CustomResourceDictionary =
                new ResourceDictionary
                {
                    Source =
                        new Uri(
                            "pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml")
                }
        };

        static My()
        {
            Rnd = new Random();

            Views = new Dictionary<View, string>
            {
                {View.SplashScreenView, "SplashScreen"},
                {View.MainView, "MainView"},
                {View.HotspotUsersView, "HotspotUsersView"},
                {View.HotspotUserProfilesView, "HotspotUserProfilesView"},
                {View.UserManagerUsersView, "UserManagerUsersView"},
                {View.UserManagerProfilesView, "UserManagerProfilesView"},
                {View.StartupView, "StartupView"},
                {View.HomeView, "HomeView"},
            };

            Regions = new Dictionary<Region, string>
            {
                {Region.MainContent, "MainContent"},
                {Region.MainRegion, "MainRegion"}
            };
        }

        public static Random Rnd { get; set; }
    }
}