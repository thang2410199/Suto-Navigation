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
using Windows.UI.Composition;

namespace SutoNavigation.Transitions
{
    public abstract class PanelTransition
    {
        public TransitionDirection Direction { get; set; } = TransitionDirection.RightToLeft;
        public TimeSpan Duration { get; set; }
        public EasingFunctionBase EasingFunction { get; set; }

        public void ResetView(ref PanelBase currentPanel, bool isGoBack)
        {
            if (!isGoBack)
            {
                var transform = currentPanel.RenderTransform as CompositeTransform;
                transform.ScaleX = transform.ScaleY = 1;
                transform.TranslateX = transform.TranslateY = 0;
                currentPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                currentPanel.Opacity = 1;
            }
        }

        /// <summary>
        /// called everytime before the transition setup happend
        /// </summary>
        /// <param name="currentPanel"></param>
        /// <param name="isGoBack"></param>
        public virtual void Setup(ref PanelBase currentPanel, bool isGoBack)
        {

        }

        /// <summary>
        /// Create list of animation will be played when navigation started
        /// </summary>
        /// <param name="currentPanel">the panel containt this transition</param>
        /// <param name="isGoBack">indicate the direction of navigation, equals true if back requested</param>
        /// <returns></returns>
        public virtual List<Timeline> CreateAnimation(ref PanelBase currentPanel, bool isGoBack)
        {
            return new List<Timeline>();
        }
        public virtual CompositionAnimationGroup CreateAnimationWithComposition(ref PanelBase panel, bool isBack)
        {
            return null;
        }

        public void ResetPreviousView(ref PanelBase currentPanel)
        {
            var stack = currentPanel.Host.PanelStack;
            var currentIndex = stack.IndexOf(currentPanel);
            if (currentIndex >= 1)
            {
                var previousPanel = stack[currentIndex - 1];
                ResetView(ref previousPanel, false);
            }

            Cleanup(ref currentPanel);
        }
        /// <summary>
        /// Overwrite this only if you made change to other panel
        /// Called when the panel is reused for difference transition, clear thing up here
        /// </summary>
        /// <param name="currentPanel"></param>
        public virtual void Cleanup(ref PanelBase currentPanel)
        {

        }

        /// <summary>
        /// Called when the parrent panel is removed from / moved in the Stack
        /// Overwrite this only if you made change to other panel
        /// </summary>
        /// <param name="previousPanel"></param>
        public virtual void SetupPreviousPanel(ref PanelBase previousPanel)
        {

        }

        public virtual void Final(ref PanelBase currentPanel)
        {

        }
    }

    public enum TransitionDirection
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft
    }

    public enum PanelType
    {
        None,
    }
}
