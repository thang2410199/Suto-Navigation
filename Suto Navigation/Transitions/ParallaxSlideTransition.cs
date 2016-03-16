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
    /// <summary>
    /// Animate next panel to parallax push the current panel in a direction
    /// </summary>
    public class ParallaxSlideTransition : PanelTransition
    {
        private PanelBase currentPanel;
        private PanelBase lastPanel;

        public ParallaxSlideTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public ParallaxSlideTransition(TimeSpan Duration, TransitionDirection direction = TransitionDirection.RightToLeft)
        {
            Direction = direction;
            this.Duration = Duration;
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
                switch (Direction)
                {
                    case TransitionDirection.LeftToRight:
                        transform.TranslateX = -currentPanel.Host.Size.Width;
                        userControl.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.System;
                        break;
                    case TransitionDirection.RightToLeft:
                        transform.TranslateX = currentPanel.Host.Size.Width;
                        userControl.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.System;
                        break;
                    case TransitionDirection.TopToBottom:
                        transform.TranslateX = -currentPanel.Host.Size.Height;
                        userControl.ManipulationMode = ManipulationModes.TranslateY | ManipulationModes.System;
                        break;
                    case TransitionDirection.BottomToTop:
                        transform.TranslateX = currentPanel.Host.Size.Height;
                        userControl.ManipulationMode = ManipulationModes.TranslateY | ManipulationModes.System;
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
            var stack = userControl.Host.PanelStack;
            if (stack.Count > 0)
            {
                lastPanel = stack[stack.Count - 2];
                var lastTransform = lastPanel.RenderTransform as CompositeTransform;

                DoubleAnimation lastAnimation = new DoubleAnimation();
                //lastAnimation.From = isBack ? -500 :0;
                if(isBack)
                {
                    lastAnimation.To = 0;
                }
                else
                {
                    switch (Direction)
                    {
                        case TransitionDirection.LeftToRight:
                        case TransitionDirection.TopToBottom:
                            lastAnimation.To = FeedbackOffset;
                            break;
                        case TransitionDirection.BottomToTop:
                        case TransitionDirection.RightToLeft:
                            lastAnimation.To = -FeedbackOffset;
                            break;
                    }
                }

                lastAnimation.Duration = userControl.Transition.Duration;
                lastAnimation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
                Storyboard.SetTarget(lastAnimation, lastTransform);
                Storyboard.SetTargetProperty(lastAnimation, base.GetTransitionProperty(Direction));
                animations.Add(lastAnimation);
            }

            var newPosition = base.GetSlideTransitionProperty(Direction, userControl.Host);
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = Duration;
            animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            animation.To = !isBack ? 0 : newPosition;
            Storyboard.SetTarget(animation, userControl.RenderTransform);
            Storyboard.SetTargetProperty(animation, base.GetTransitionProperty(Direction));
            animations.Add(animation);

            return animations;
        }

        public override void ResetOnReUse(ref PanelBase userControl)
        {
            var lastTransform = lastPanel.RenderTransform as CompositeTransform;
            lastTransform.TranslateX = lastTransform.TranslateY = 0;
            UnregisterManipulation(ref userControl);
        }

        public override void SetLastPanelInitialState(ref PanelBase lastUserControl)
        {
            var transform = lastUserControl.RenderTransform as CompositeTransform;
            switch (Direction)
            {
                case TransitionDirection.LeftToRight:
                    transform.TranslateX = FeedbackOffset;
                    break;
                case TransitionDirection.RightToLeft:
                    transform.TranslateX = -FeedbackOffset;
                    break;
                case TransitionDirection.TopToBottom:
                    transform.TranslateY = -FeedbackOffset;
                    break;
                case TransitionDirection.BottomToTop:
                    transform.TranslateY = FeedbackOffset;
                    break;

            }
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

            var currentPostion = transform.TranslateX;
            switch (Direction)
            {
                case TransitionDirection.BottomToTop:
                case TransitionDirection.TopToBottom:
                    currentPostion = transform.TranslateY;
                    break;
                case TransitionDirection.LeftToRight:
                case TransitionDirection.RightToLeft:
                    currentPostion = transform.TranslateX;
                    break;
            }

            var feedbackOffset = FeedbackOffset;
            //switch (Direction)
            //{
            //    case TransitionDirection.BottomToTop:
            //    case TransitionDirection.LeftToRight:
            //        feedbackOffset = -FeedbackOffset;
            //        break;
            //}

            if (Math.Abs(currentPostion) < feedbackOffset)
            {
                //Move it back to 0

                DoubleAnimation animation = new DoubleAnimation();
                animation.To = 0;
                //Set the duration to relect how much they had moved the panel
                animation.Duration = TimeSpan.FromMilliseconds(Duration.TotalMilliseconds * currentPostion / currentPanel.Host.Size.Width);
                QuadraticEase ease = new QuadraticEase();
                ease.EasingMode = EasingMode.EaseOut;
                animation.EasingFunction = ease;

                Storyboard storyboard = new Storyboard();
                Storyboard.SetTarget(animation, transform);
                Storyboard.SetTargetProperty(animation, base.GetTransitionProperty(this.Direction));
                storyboard.Children.Add(animation);
                storyboard.Begin();

            }
            else
            {
                //Invoke back event.
                currentPanel.Host.GoBack();
            }
        }

        private void UserControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var transform = currentPanel.RenderTransform as CompositeTransform;
            var newPosition = transform.TranslateX;
            double delta = 0;
            double limit = 0;
            var feedbackOffset = FeedbackOffset;
            switch (Direction)
            {
                case TransitionDirection.BottomToTop:
                case TransitionDirection.TopToBottom:
                    newPosition = transform.TranslateY;
                    delta = e.Delta.Translation.Y;
                    break;
                case TransitionDirection.LeftToRight:
                case TransitionDirection.RightToLeft:
                    newPosition = transform.TranslateX;
                    delta = e.Delta.Translation.X;
                    break;
            }

            newPosition += delta;
            bool moved = false;
            if (Direction == TransitionDirection.RightToLeft || Direction == TransitionDirection.BottomToTop)
            {
                if (newPosition < limit)
                    newPosition = limit;
                else
                {
                    moved = true;
                }
            }
            else
            {
                if (newPosition > limit)
                    newPosition = limit;
                else
                {
                    moved = true;
                }
                feedbackOffset = -feedbackOffset;
            }

            if(moved)
            {
                var stack = currentPanel.Host.PanelStack;
                lastPanel = stack[stack.Count - 2];
                var lastTransform = lastPanel.RenderTransform as CompositeTransform;

                switch (Direction)
                {
                    case TransitionDirection.LeftToRight:
                    case TransitionDirection.RightToLeft:
                        lastTransform.TranslateX = (1 - Math.Abs(newPosition / currentPanel.Host.Size.Width)) * -feedbackOffset;
                        break;
                    case TransitionDirection.TopToBottom:
                    case TransitionDirection.BottomToTop:
                        lastTransform.TranslateY = (1 - Math.Abs(newPosition / currentPanel.Host.Size.Height)) * -feedbackOffset;
                        break;

                }
            }

            switch (Direction)
            {
                case TransitionDirection.BottomToTop:
                case TransitionDirection.TopToBottom:
                    transform.TranslateY = newPosition;
                    break;
                case TransitionDirection.LeftToRight:
                case TransitionDirection.RightToLeft:
                    transform.TranslateX = newPosition;
                    break;
            }

        }
    }
}
