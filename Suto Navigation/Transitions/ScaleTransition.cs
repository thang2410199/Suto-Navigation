using SutoNavigation.NavigationService;
using SutoNavigation.NavigationService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SutoNavigation.Transitions
{
    /// <summary>
    /// Animate next panel to scale from center of current panel to 1.
    /// </summary>
    public class ScaleTransition : PanelTransition
    {
        double initialScale = 0.65;
        public ScaleTransition(TimeSpan duration, double initialScale)
        {
            Duration = duration;
            this.initialScale = initialScale;
        }

        public ScaleTransition(TimeSpan duration)
        {
            Duration = duration;
        }

        public ScaleTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override void Setup(ref PanelBase userControl, bool isBack)
        {
            if (!isBack)
            {
                var render = userControl.RenderTransform as CompositeTransform;
                userControl.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
                render.ScaleX = render.ScaleY = initialScale;
                userControl.Opacity = 0;
            }
        }

        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isBack)
        {
            DoubleAnimation animationX = new DoubleAnimation();
            animationX.Duration = Duration;
            animationX.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animationX.To = !isBack ? 1 : initialScale;
            Storyboard.SetTarget(animationX, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animationX, nameof(CompositeTransform.ScaleX));

            DoubleAnimation animationY = new DoubleAnimation();
            animationY.Duration = Duration;
            animationY.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animationY.To = !isBack ? 1 : initialScale;
            Storyboard.SetTarget(animationY, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animationY, nameof(CompositeTransform.ScaleY));

            DoubleAnimation animationOpacity = new DoubleAnimation();
            animationOpacity.Duration = Duration;
            animationOpacity.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animationOpacity.From = userControl.Opacity;
            animationOpacity.To = isBack ? 0 : 1;
            Storyboard.SetTarget(animationOpacity, userControl);
            Storyboard.SetTargetProperty(animationOpacity, nameof(userControl.Opacity));

            return new List<Timeline>() { animationX, animationY, animationOpacity };
        }
    }
}
