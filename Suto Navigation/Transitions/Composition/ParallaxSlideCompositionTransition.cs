using SutoNavigation.Helpers;
using SutoNavigation.NavigationService;
using SutoNavigation.NavigationService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SutoNavigation.Transitions
{
    /// <summary>
    /// Animate next panel to parallax push the current panel in a direction
    /// </summary>
    public class ParallaxSlideCompositionTransition : PanelTransition
    {
        private PanelBase currentPanel;
        private PanelBase lastPanel;
        private bool initialized = false;
        private bool gestureSupport = true;

        public ParallaxSlideCompositionTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public ParallaxSlideCompositionTransition(TimeSpan Duration, TransitionDirection direction = TransitionDirection.RightToLeft)
        {
            Direction = direction;
            this.Duration = Duration;
        }

        public ParallaxSlideCompositionTransition(TimeSpan Duration, TransitionDirection direction, bool gestureEnable = true)
        {
            Direction = direction;
            this.Duration = Duration;
            this.gestureSupport = gestureEnable;
        }

        /// <summary>
        /// Set the distance the last panel will move when using Instagram transition. Default value 100
        /// </summary>
        public double FeedbackOffset { get; set; } = 100;

        public override void Setup(ref PanelBase userControl, bool isGoBack)
        {
            base.Setup(ref userControl, isGoBack);
            if (!isGoBack)
            {
                currentPanel = userControl;
                switch (Direction)
                {
                    case TransitionDirection.LeftToRight:
                        currentPanel.Visual.Offset = new Vector3(-(float)currentPanel.Host.Size.Width, 0, 0);
                        userControl.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.System;
                        break;
                    case TransitionDirection.RightToLeft:
                        currentPanel.Visual.Offset = new Vector3((float)currentPanel.Host.Size.Width, 0, 0);
                        userControl.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.System;
                        break;
                    case TransitionDirection.TopToBottom:
                        currentPanel.Visual.Offset = new Vector3(0, -(float)currentPanel.Host.Size.Width, 0);
                        userControl.ManipulationMode = ManipulationModes.TranslateY | ManipulationModes.System;
                        break;
                    case TransitionDirection.BottomToTop:
                        currentPanel.Visual.Offset = new Vector3(0, (float)currentPanel.Host.Size.Width, 0);
                        userControl.ManipulationMode = ManipulationModes.TranslateY | ManipulationModes.System;
                        break;

                }
                if (gestureSupport)
                    RegisterManipulation(ref userControl);
            }
            else
            {
                if (gestureSupport)
                    UnregisterManipulation(ref userControl);
            }
        }

        public override List<(CompositionAnimationGroup animationGroup, IPanel panel)> CreateAnimationWithComposition(ref PanelBase panel, bool isBack)
        {
            var result = new List<(CompositionAnimationGroup animationGroup, IPanel panel)>();
            var animations = panel.Compositor.CreateAnimationGroup();
            var stack = panel.Host.PanelStack;
            if (stack.Count > 0)
            {
                lastPanel = stack[stack.Count - 2];
                var lastAnimationGroup = lastPanel.Compositor.CreateAnimationGroup();
                var lastAnimation = lastPanel.Compositor.CreateVector3KeyFrameAnimation();
                lastAnimation.Target = nameof(Visual.Offset);
                lastAnimation.Duration = this.Duration;
                lastAnimation.InsertKeyFrame(0f, lastPanel.Visual.Offset);
                Vector3 targetOffset = lastPanel.Visual.Offset;
                //lastAnimation.From = isBack ? -500 :0;
                if (isBack)
                {
                    targetOffset = Vector3.Zero;
                }
                else
                {
                    switch (Direction)
                    {
                        case TransitionDirection.LeftToRight:
                            targetOffset.X = (float)FeedbackOffset;
                            break;
                        case TransitionDirection.RightToLeft:
                            targetOffset.X = -(float)FeedbackOffset;
                            break;
                        case TransitionDirection.TopToBottom:
                            targetOffset.Y = (float)FeedbackOffset;
                            break;
                        case TransitionDirection.BottomToTop:
                            targetOffset.Y = -(float)FeedbackOffset;
                            break;
                    }
                }

                lastAnimation.InsertKeyFrame(1f, targetOffset, SutoEasingFunction.EaseOut(lastPanel.Compositor));
                lastAnimationGroup.Add(lastAnimation);

                result.Add((lastAnimationGroup, lastPanel));
            }

            var newPosition = SlideTransitionHelper.GetSlideTargetValue(Direction, panel.Host);
            var newOffset = panel.Visual.Offset;
            var animation = panel.Compositor.CreateVector3KeyFrameAnimation();
            animation.Duration = this.Duration;
            animation.Target = nameof(Visual.Offset);
            animation.InsertKeyFrame(0f, panel.Visual.Offset);
            if(isBack)
            {
                switch (Direction)
                {
                    case TransitionDirection.LeftToRight:
                        newOffset.X = -(float)newPosition;
                        break;
                    case TransitionDirection.RightToLeft:
                        newOffset.X = (float)newPosition;
                        break;
                    case TransitionDirection.TopToBottom:
                        newOffset.Y = -(float)newPosition;
                        break;
                    case TransitionDirection.BottomToTop:
                        newOffset.Y = (float)newPosition;
                        break;
                }
            }
            else
            {
                newOffset = Vector3.Zero;
            }
            animation.InsertKeyFrame(1f, newOffset, SutoEasingFunction.EaseOut(panel.Compositor));
            animations.Add(animation);

            result.Add((animations, panel));

            return result;
        }

        public override void Cleanup(ref PanelBase userControl)
        {
            lastPanel.Visual.Offset = new Vector3(0);
            if (gestureSupport)
                UnregisterManipulation(ref userControl);
        }

        public override void Final(ref PanelBase currentPanel)
        {
            base.Final(ref currentPanel);

            currentPanel.Visual.Offset = new Vector3(0);
        }

        public override void SetupPreviousPanel(ref PanelBase lastUserControl)
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
            if (!initialized)
            {
                userControl.ManipulationDelta += UserControl_ManipulationDelta;
                userControl.ManipulationCompleted += UserControl_ManipulationCompleted;
                initialized = true;
            }
        }

        private void UnregisterManipulation(ref PanelBase userControl)
        {
            if (initialized)
            {
                userControl.ManipulationDelta -= UserControl_ManipulationDelta;
                userControl.ManipulationCompleted -= UserControl_ManipulationCompleted;
                initialized = false;
            }
        }

        private void UserControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var currentPostion = currentPanel.Visual.Offset.X;
            switch (Direction)
            {
                case TransitionDirection.BottomToTop:
                case TransitionDirection.TopToBottom:
                    currentPostion = currentPanel.Visual.Offset.Y;
                    break;
                case TransitionDirection.LeftToRight:
                case TransitionDirection.RightToLeft:
                    currentPostion = currentPanel.Visual.Offset.X;
                    break;
            }

            var feedbackOffset = FeedbackOffset;

            if (Math.Abs(currentPostion) < feedbackOffset)
            {
                //Move it back to 0
                var animation = currentPanel.Compositor.CreateVector3KeyFrameAnimation();
                animation.InsertKeyFrame(0, currentPanel.Visual.Offset);
                animation.InsertKeyFrame(1, Vector3.Zero, SutoEasingFunction.EaseOut(currentPanel.Compositor));
                animation.Target = nameof(Visual.Offset);
                animation.Duration = TimeSpan.FromMilliseconds(Duration.TotalMilliseconds * currentPostion / currentPanel.Host.Size.Width);
                currentPanel.Visual.StartAnimation(nameof(Visual.Offset), animation);
            }
            else
            {
                //Invoke back event.
                currentPanel.Host.GoBack();
            }
        }

        private void UserControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            double newPosition = currentPanel.Visual.Offset.X;
            double delta = 0;
            double limit = 0;
            var feedbackOffset = FeedbackOffset;
            switch (Direction)
            {
                case TransitionDirection.BottomToTop:
                case TransitionDirection.TopToBottom:
                    newPosition = currentPanel.Visual.Offset.Y;
                    delta = e.Delta.Translation.Y;
                    break;
                case TransitionDirection.LeftToRight:
                case TransitionDirection.RightToLeft:
                    newPosition = currentPanel.Visual.Offset.X;
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

            if (moved)
            {
                var stack = currentPanel.Host.PanelStack;
                lastPanel = stack[stack.Count - 2];

                switch (Direction)
                {
                    case TransitionDirection.LeftToRight:
                    case TransitionDirection.RightToLeft:
                        var newX = (1 - Math.Abs(newPosition / currentPanel.Host.Size.Width)) * -feedbackOffset;
                        lastPanel.Visual.Offset = new Vector3((float)newX, lastPanel.Visual.Offset.Y, lastPanel.Visual.Offset.Z);
                        break;
                    case TransitionDirection.TopToBottom:
                    case TransitionDirection.BottomToTop:
                        var newY = (1 - Math.Abs(newPosition / currentPanel.Host.Size.Height)) * -feedbackOffset;
                        lastPanel.Visual.Offset = new Vector3(lastPanel.Visual.Offset.X, (float)newY, lastPanel.Visual.Offset.Z);
                        break;

                }
            }

            switch (Direction)
            {
                case TransitionDirection.BottomToTop:
                case TransitionDirection.TopToBottom:
                    if (currentPanel.Visual.Offset.Y != newPosition)
                        e.Handled = true;
                    currentPanel.Visual.Offset = new Vector3(currentPanel.Visual.Offset.X, (float)newPosition, currentPanel.Visual.Offset.Z);
                    break;
                case TransitionDirection.LeftToRight:
                case TransitionDirection.RightToLeft:
                    if (currentPanel.Visual.Offset.X != newPosition)
                        e.Handled = true;
                    currentPanel.Visual.Offset = new Vector3((float)newPosition, currentPanel.Visual.Offset.Y, currentPanel.Visual.Offset.Z);
                    break;
            }
        }
    }
}