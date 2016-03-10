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
        public FadeInTransition(TimeSpan duration)
        {
            Duration = duration;
        }

        public FadeInTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override void SetInitialState(ref PanelBase userControl, bool isBack)
        {
            if(!isBack)
                userControl.Opacity = 0;
        }

        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isBack)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = Duration;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animation.From = userControl.Opacity;
            animation.To = isBack ? 0 : 1;
            Storyboard.SetTarget(animation, userControl);
            Storyboard.SetTargetProperty(animation, nameof(userControl.Opacity));


            return new List<Timeline>() { animation };
        }
    }
}
