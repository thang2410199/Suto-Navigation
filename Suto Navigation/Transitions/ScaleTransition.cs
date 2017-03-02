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
        double endScale = 1;
        public ScaleTransition(TimeSpan duration, double initialScale, double endScale)
        {
            Duration = duration;
            this.initialScale = initialScale;
            this.endScale = endScale;
        }

        public ScaleTransition(TimeSpan duration)
        {
            Duration = duration;
        }

        public ScaleTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override void Setup(ref PanelBase currentPanel, bool isGoBack)
        {
            base.Setup(ref currentPanel, isGoBack);
            if (!isGoBack)
            {
                var render = currentPanel.RenderTransform as CompositeTransform;
                currentPanel.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
                render.ScaleX = render.ScaleY = initialScale;
            }
        }

        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isGoBack)
        {
            DoubleAnimation animationX = new DoubleAnimation();
            animationX.Duration = Duration;
            animationX.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animationX.To = !isGoBack ? endScale : initialScale;
            Storyboard.SetTarget(animationX, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animationX, nameof(CompositeTransform.ScaleX));

            DoubleAnimation animationY = new DoubleAnimation();
            animationY.Duration = Duration;
            animationY.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animationY.To = !isGoBack ? endScale : initialScale;
            Storyboard.SetTarget(animationY, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animationY, nameof(CompositeTransform.ScaleY));

            return new List<Timeline>() { animationX, animationY };
        }

        public override void Final(ref PanelBase currentPanel)
        {
            base.Final(ref currentPanel);

            var render = currentPanel.RenderTransform as CompositeTransform;
            render.ScaleX = render.ScaleY = endScale;
        }
    }
}
