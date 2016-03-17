using SutoNavigation.Interfaces;
using SutoNavigation.Transitions;
using Windows.UI.Xaml.Media;

namespace SutoNavigation.Helpers
{
    public class SlideTransitionHelper
    {
        public static double GetSlideTargetValue(TransitionDirection direction, IPanelHost host)
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

        public static string GetSlideTargetPropertyName(TransitionDirection direction)
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
}
