using SutoNavigation.NavigationService;
using Windows.UI.Input;
using Windows.UI.Xaml;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Suto_Navigation.Sample.Panes
{
    public sealed partial class ProfilePanel : PanelBase
    {
        public ProfilePanel()
        {
            this.InitializeComponent();

            GestureRecognizer input = new GestureRecognizer();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.host.Navigate(typeof(LoginPanel), null, new PanelTransition(PanelTransitionType.InstagramLike));
        }
    }
}
