using SutoNavigation.Helpers;
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
    /// Animate next panel to overlay the current panel in a direction
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
        }

        public SlideInTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override void Setup(ref PanelBase currentPanel, bool isGoBack)
        {
            base.Setup(ref currentPanel, isGoBack);
            if (!isGoBack)
            {
                var render = currentPanel.RenderTransform as CompositeTransform;
                var newPosition = SlideTransitionHelper.GetSlideTargetValue(Direction, currentPanel.Host);
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
            var newPosition = SlideTransitionHelper.GetSlideTargetValue(Direction, userControl.Host);
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = Duration;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animation.To = !isBack ? 0 : newPosition;
            Storyboard.SetTarget(animation, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animation, SlideTransitionHelper.GetSlideTargetPropertyName(Direction));


            return new List<Timeline>() { animation };
        }
    }
}
