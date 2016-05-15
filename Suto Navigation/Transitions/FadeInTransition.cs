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
        double initialOpacity = 0;
        double endOpacity = 1;
        public FadeInTransition(TimeSpan duration, double initialOpacity, double endOpacity)
        {
            this.Duration = duration;
            this.initialOpacity = initialOpacity;
            this.endOpacity = endOpacity;
        }
        public FadeInTransition(TimeSpan duration)
        {
            Duration = duration;
        }

        public FadeInTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override void Setup(ref PanelBase userControl, bool isGoBack)
        {
            base.Setup(ref userControl, isGoBack);
            if(!isGoBack)
                userControl.Opacity = initialOpacity;
        }

        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isGoBack)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = Duration;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animation.From = userControl.Opacity;
            animation.To = isGoBack ? initialOpacity : endOpacity;
            Storyboard.SetTarget(animation, userControl);
            Storyboard.SetTargetProperty(animation, nameof(userControl.Opacity));


            return new List<Timeline>() { animation };
        }
    }
}
