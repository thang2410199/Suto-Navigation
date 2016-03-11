using SutoNavigation.NavigationService;
using SutoNavigation.Transitions;
using System;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Suto_Navigation.Sample.Panes
{
    public sealed partial class ProviderPanel : PanelBase
    {
        public ProviderPanel()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Host.Navigate(typeof(LoginPanel), null, new SlideInTransition(TimeSpan.FromSeconds(1)));
        }
    }
}
