using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;

namespace Suto_Navigation_Composition
{
    public class TransitionFrame : Frame
    {
        Visual current_visual;

        public TransitionFrame() : base()
        {
            this.Navigated += TransitionFrame_Navigated;
            this.Navigating += TransitionFrame_Navigating;
        }

        private void TransitionFrame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            if (this.Content != null)
            {
                current_visual = ElementCompositionPreview.GetElementVisual(Content as UIElement);
            }
        }

        private void TransitionFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var page = (e.Content as Page);
            var new_visual = ElementCompositionPreview.GetElementVisual(page);
            Debug.WriteLine("Navigated");
        }

        public bool TransitionTo(Type type, object parameter)
        {
            return Navigate(type, parameter);
        }
    }
}
