using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace EZ_TIK.Resources
{
    public class FaderFrame : Frame
    {
        private ContentPresenter _contentPresnter;
        private NavigatingCancelEventArgs _navArgs;
        private bool _allowDirectNavigation;

        #region Fade Duration

        public static readonly DependencyProperty FadeDurationProperty = DependencyProperty.Register(
            "FadeDuration", typeof(Duration), typeof(FaderFrame), new PropertyMetadata(default(Duration)));



        public Duration FadeDuration
        {
            get => (Duration)GetValue(FadeDurationProperty); set => SetValue(FadeDurationProperty, value);
        }

        #endregion

        #region Ctors

        public FaderFrame() : base()
        {
            Navigating += OnNavigating;
        }

        #endregion

        #region Event Handlers
        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content != null && !_allowDirectNavigation && _contentPresnter != null)
            {
                e.Cancel = true;
                _navArgs = e;

                _contentPresnter.IsHitTestVisible = false;
                var da = new DoubleAnimation(0.0d, FadeDuration) { DecelerationRatio = 1.0d };

                da.Completed += FadeOutCompleted;
                _contentPresnter.BeginAnimation(OpacityProperty, da);
            }

            _allowDirectNavigation = false;
        }

        private void FadeOutCompleted(object sender, EventArgs e)
        {
            ((AnimationClock) sender).Completed -= FadeOutCompleted;

            if (_contentPresnter == null) return;

            _contentPresnter.IsHitTestVisible = true;
            _allowDirectNavigation = true;

            switch (_navArgs.NavigationMode)
            {
                case NavigationMode.New:
                    if (_navArgs.Uri == null)
                    {
                        NavigationService.Navigate(_navArgs.Content);
                    }
                    else
                    {
                        NavigationService.Navigate(_navArgs.Uri);
                    }
                    break;
                case NavigationMode.Back:
                    NavigationService.GoBack();
                    break;
                case NavigationMode.Forward:
                    NavigationService.GoForward();
                    break;
                case NavigationMode.Refresh:
                    NavigationService.Refresh();
                    break;
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)delegate
            {
                var da = new DoubleAnimation(1.0d, FadeDuration) { AccelerationRatio = 1.0d };
                _contentPresnter.BeginAnimation(OpacityProperty, da);
            });
        }

        #endregion

        #region Overridden Methods

        public override void OnApplyTemplate()
        {
            _contentPresnter = GetTemplateChild("PART_FrameCP") as ContentPresenter;
            base.OnApplyTemplate();
        }

        #endregion
    }
}
