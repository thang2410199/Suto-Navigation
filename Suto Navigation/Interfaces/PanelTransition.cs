using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace SutoNavigation.NavigationService
{
    public enum PanelTransitionType
    {
        None,
        FadeIn,
        SlideIn,
        BounceIn,
        InstagramLike,
    }

    public class PanelTransition
    {
        public PanelTransitionType Type { get; set; }
        public TransitionDirection Direction { get; set; }
        public TimeSpan Duration { get; set; }
        public EasingFunctionBase EasingFunction { get; set; }
        public PanelTransition(PanelTransitionType type, TransitionDirection direction = TransitionDirection.RightToLeft)
        {
            Type = type;
            Direction = direction;
            Duration = TimeSpan.FromMilliseconds(400);
        }

        public PanelTransition(PanelTransitionType type, TimeSpan duration, TransitionDirection direction = TransitionDirection.RightToLeft)
        {
            Type = type;
            Direction = direction;
            Duration = duration;
        }

        public PanelTransition()
        {
            Type = PanelTransitionType.None;
            Duration = TimeSpan.FromMilliseconds(400);
        }
    }

    public enum TransitionDirection
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft
    }

    public enum State
    {
        Idle,
        FadingIn,
        FadingOut,
        Transiting,
    }

    public enum PanelType
    {
        None,
    }
}
