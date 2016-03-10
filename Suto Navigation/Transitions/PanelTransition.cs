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
    public abstract class PanelTransition
    {
        public TransitionDirection Direction { get; set; }
        public TimeSpan Duration { get; set; }
        public EasingFunctionBase EasingFunction { get; set; }

        public virtual void SetInitialState(ref PanelBase userControl, bool isBack)
        {

        }

        public virtual List<Timeline> CreateAnimation(ref PanelBase userControl, bool isBack)
        {
            return new List<Timeline>();
        }

        internal double GetSlideTransitionProperty(TransitionDirection direction, IPanelHost host)
        {
            switch (direction)
            {
                case TransitionDirection.BottomToTop:
                    return host.Size.Height;
                case TransitionDirection.TopToBottom:
                    return -host.Size.Height;
                case TransitionDirection.LeftToRight:
                    return -host.Size.Width;
                case TransitionDirection.RightToLeft:
                    return host.Size.Width;
            }
            return host.Size.Width;
        }

        internal string GetTransitionProperty(TransitionDirection direction)
        {
            switch (direction)
            {
                case TransitionDirection.BottomToTop:
                case TransitionDirection.TopToBottom:
                    return nameof(CompositeTransform.TranslateY);
                case TransitionDirection.LeftToRight:
                case TransitionDirection.RightToLeft:
                    return nameof(CompositeTransform.TranslateX);
            }
            return nameof(CompositeTransform.TranslateX);
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
