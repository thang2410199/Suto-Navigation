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
    /// Animate next panel to push the current panel in a direction
    /// </summary>
    public class SlidePushTransition : PanelTransition
    {
        public SlidePushTransition(TimeSpan duration, TransitionDirection direction)
        {
            Duration = duration;
            Direction = direction;
        }

        public SlidePushTransition(TimeSpan duration)
        {
            Duration = duration;
        }

        public SlidePushTransition()
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
        PanelBase lastPanel;
        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isBack)
        {
            var animations = new List<Timeline>();
            var newPosition = base.GetSlideTransitionProperty(Direction, userControl.Host);
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = Duration;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animation.To = !isBack ? 0 : newPosition;
            Storyboard.SetTarget(animation, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animation, base.GetTransitionProperty(Direction));

            animations.Add(animation);

            var stack = userControl.Host.PanelStack;
            if (stack.Count > 0)
            {
                lastPanel = stack[stack.Count - 2];
                DoubleAnimation subAnimation = new DoubleAnimation();
                subAnimation.Duration = Duration;
                subAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
                subAnimation.To = isBack ? 0 : -newPosition;
                Storyboard.SetTarget(subAnimation, lastPanel.RenderTransform);
                Storyboard.SetTargetProperty(subAnimation, base.GetTransitionProperty(Direction));

                animations.Add(subAnimation);
            }




            return animations;
        }

        public override void ResetOnReUse(ref PanelBase userControl)
        {
            var lastTransform = lastPanel.RenderTransform as CompositeTransform;
            lastTransform.TranslateX = lastTransform.TranslateY = 0;
        }

        public override void SetLastPanelInitialState(ref PanelBase lastUserControl)
        {
            var transform = lastUserControl.RenderTransform as CompositeTransform;
            var newPosition = base.GetSlideTransitionProperty(Direction, lastUserControl.Host);
        }
    }
}
