using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SutoNavigation.NavigationService;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media;
using SutoNavigation.Helpers;

namespace SutoNavigation.Transitions
{
    /// <summary>
    /// previous panel scale down and fade away while new panel slide in
    /// </summary>
    public class ScaleSlideTransition : PanelTransition
    {
        PanelBase previousPanel;
        public ScaleSlideTransition()
        {
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public override void Setup(ref PanelBase currentPanel, bool isGoBack)
        {
            base.Setup(ref currentPanel, isGoBack);
            if (!isGoBack)
            {
                previousPanel = currentPanel.Host.PanelStack.Take(currentPanel.Host.PanelStack.Count - 1).LastOrDefault();
                var render = previousPanel.RenderTransform as CompositeTransform;
                previousPanel.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
                render.ScaleX = render.ScaleY = 1;

                render = currentPanel.RenderTransform as CompositeTransform;
                var newPosition = SlideTransitionHelper.GetSlideTargetValue(Direction, currentPanel.Host);
                switch (Direction)
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

        public override List<Timeline> CreateAnimation(ref PanelBase currentPanel, bool isGoBack)
        {
            var animations = new List<Timeline>();


            var slideAnimation = new SlideInTransition(Duration, Direction).CreateAnimation(ref currentPanel, isGoBack);

            animations.AddRange(slideAnimation);

            var fadeAnimation = new FadeInTransition(Duration, 1, 0).CreateAnimation(ref previousPanel, isGoBack);

            animations.AddRange(fadeAnimation);

            var scaleAnimation = new ScaleTransition(Duration, 1, 0.9).CreateAnimation(ref previousPanel, isGoBack);

            animations.AddRange(scaleAnimation);

            return animations;
        }

        public override void SetupPreviousPanel(ref PanelBase previousPanel)
        {
            base.SetupPreviousPanel(ref previousPanel);
        }
    }
}
