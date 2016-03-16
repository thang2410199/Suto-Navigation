using SutoNavigation.NavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace SutoNavigation.Transitions
{
    /// <summary>
    /// Animate Opacity of the Panel
    /// </summary>
    public class FadeInTransition : PanelTransition
    {
        double initialScale = 0;
        public FadeInTransition(TimeSpan duration, double initialScale)
        {
            this.Duration = duration;
            this.initialScale = initialScale;
        }
        public FadeInTransition(TimeSpan duration)
        {
            Duration = duration;
        }

        public FadeInTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override void Setup(ref PanelBase userControl, bool isBack)
        {
            if(!isBack)
                userControl.Opacity = initialScale;
        }

        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isBack)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = Duration;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animation.From = userControl.Opacity;
            animation.To = isBack ? initialScale : 1;
            Storyboard.SetTarget(animation, userControl);
            Storyboard.SetTargetProperty(animation, nameof(userControl.Opacity));


            return new List<Timeline>() { animation };
        }
    }
}
