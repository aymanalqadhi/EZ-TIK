﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace EZ_TIK.AttachedProperties
{
    public static class FrameAttachedProperties
    {
        #region Disable Navigation Property

        public static bool GetDisable(DependencyObject o)
        {
            var value = o.GetValue(DisableProperty);
            return value != null && (bool)value;
        }

        public static void SetDisable(DependencyObject o, bool value)
        {
            o.SetValue(DisableProperty, value);
        }

        public static readonly DependencyProperty DisableProperty =
            DependencyProperty.RegisterAttached("Disable", typeof(bool), typeof(FrameAttachedProperties),
                                                new PropertyMetadata(false, DisableChanged));

        public static void DisableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var frame = (Frame)sender;
            frame.Navigated += DontNavigate;
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        public static void DontNavigate(object sender, NavigationEventArgs e)
        {
            ((Frame)sender).NavigationService.RemoveBackEntry();
        } 

        #endregion
    }
}