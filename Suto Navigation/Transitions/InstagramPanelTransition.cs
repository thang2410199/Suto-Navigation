using SutoNavigation.NavigationService;
using SutoNavigation.NavigationService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SutoNavigation.Transitions
{
    public class InstagramPanelTransition : PanelTransition
    {
        private PanelBase currentPanel;
        private PanelBase lastPanel;

        public InstagramPanelTransition()
        {
            Direction = TransitionDirection.RightToLeft;
            Duration = TimeSpan.FromMilliseconds(4000);
        }

        /// <summary>
        /// Set the distance the last panel will move when using Instagram transition. Default value 100
        /// </summary>
        public double FeedbackOffset { get; set; } = 100;

        public override void SetInitialState(ref PanelBase userControl, bool isBack)
        {
            if (!isBack)
            {
                currentPanel = userControl;
                var transform = currentPanel.RenderTransform as CompositeTransform;
                switch(Direction)
                {
                    case TransitionDirection.LeftToRight:
                        transform.TranslateX = -currentPanel.host.Size.Width;
                        break;
                    case TransitionDirection.RightToLeft:
                        transform.TranslateX = currentPanel.host.Size.Width;
                        break;
                    case TransitionDirection.TopToBottom:
                        transform.TranslateX = -currentPanel.host.Size.Height;
                        break;
                    case TransitionDirection.BottomToTop:
                        transform.TranslateX = currentPanel.host.Size.Height;
                        break;

                }
                RegisterManipulation(ref userControl);
            }
            else
            {
                UnregisterManipulation(ref userControl);
            }
        }

        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isBack)
        {
            var animations = new List<Timeline>();
            var stack = userControl.host.PanelStack;
            if (stack.Count > 0)
            {
                lastPanel = (isBack ? stack[stack.Count - 2] : stack.Last());
                var lastTransform = lastPanel.RenderTransform as CompositeTransform;

                DoubleAnimation lastAnimation = new DoubleAnimation();
                //lastAnimation.From = isBack ? -500 :0;
                lastAnimation.To = isBack ? 0 : -FeedbackOffset;
                lastAnimation.Duration = userControl.Transition.Duration;
                lastAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
                Storyboard.SetTarget(lastAnimation, lastTransform);
                Storyboard.SetTargetProperty(lastAnimation, base.GetTransitionProperty(Direction));
                animations.Add(lastAnimation);
            }

            var newPosition = base.GetSlideTransitionProperty(Direction, userControl.host);
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = Duration;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animation.To = !isBack ? 0 : newPosition;
            Storyboard.SetTarget(animation, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animation, base.GetTransitionProperty(Direction));
            animations.Add(animation);

            if (isBack)
                UnregisterManipulation(ref currentPanel);
            else
                RegisterManipulation(ref currentPanel);



            return animations;
        }

        private void RegisterManipulation(ref PanelBase userControl)
        {
            userControl.ManipulationDelta += UserControl_ManipulationDelta;
            userControl.ManipulationCompleted += UserControl_ManipulationCompleted;
        }

        private void UnregisterManipulation(ref PanelBase userControl)
        {
            userControl.ManipulationDelta -= UserControl_ManipulationDelta;
            userControl.ManipulationCompleted -= UserControl_ManipulationCompleted;
        }

        private void UserControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var transform = currentPanel.RenderTransform as CompositeTransform;
            if (transform.TranslateX < FeedbackOffset)
            {
                //Move it back to 0

                DoubleAnimation animation = new DoubleAnimation();
                animation.To = 0;
                animation.Duration = TimeSpan.FromMilliseconds(200);
                QuadraticEase ease = new QuadraticEase();
                ease.EasingMode = EasingMode.EaseOut;
                animation.EasingFunction = ease;

                Storyboard storyboard = new Storyboard();
                Storyboard.SetTarget(animation, transform);
                Storyboard.SetTargetProperty(animation, "TranslateX");
                storyboard.Children.Add(animation);
                storyboard.Begin();

            }
            else
            {
                //Invoke back event.
                currentPanel.host.GoBack();
            }
        }

        private void UserControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var transform = currentPanel.RenderTransform as CompositeTransform;
            var newX = transform.TranslateX;
            newX += e.Delta.Translation.X;
            if (newX < 0)
                newX = 0;
            else
            {
                var stack = currentPanel.host.PanelStack;
                lastPanel = stack[stack.Count - 2];
                var lastTransform = lastPanel.RenderTransform as CompositeTransform;

                lastTransform.TranslateX = (1 - newX / currentPanel.host.Size.Width) * -FeedbackOffset;
            }

            transform.TranslateX = newX;
        }
    }
}
