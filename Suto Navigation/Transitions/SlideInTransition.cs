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
    /// Animate Opacity of the Panel
    /// </summary>
    public class SlideInTransition : PanelTransition
    {
        public SlideInTransition(TimeSpan duration, TransitionDirection direction)
        {
            Duration = duration;
            Direction = direction;
        }

        public SlideInTransition(TimeSpan duration)
        {
            Duration = duration;
            Direction = TransitionDirection.RightToLeft;
        }

        public SlideInTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override void SetInitialState(ref PanelBase userControl, bool isBack)
        {
            if(!isBack)
            {
                var render = userControl.RenderTransform as CompositeTransform;
                var newPosition = base.GetSlideTransitionProperty(Direction, userControl.Host);
                switch(Direction)
                {
                    case TransitionDirection.RightToLeft:
                    case TransitionDirection.LeftToRight:
                        render.TranslateX = newPosition;
                        break;
                    case TransitionDirection.BottomToTop:
                    case TransitionDirection.TopToBottom:
                        render.TranslateY = newPosition;
                        break;
                }
            }
        }

        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isBack)
        {
            var newPosition = base.GetSlideTransitionProperty(Direction, userControl.Host);
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = Duration;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animation.To = !isBack ? 0 : newPosition;
            Storyboard.SetTarget(animation, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animation, base.GetTransitionProperty(Direction));


            return new List<Timeline>() { animation };
        }
    }
}
