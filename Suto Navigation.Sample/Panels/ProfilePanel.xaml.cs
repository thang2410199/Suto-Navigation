using SutoNavigation;
using SutoNavigation.NavigationService;
using SutoNavigation.Transitions;
using System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Suto_Navigation.Sample.Panes
{
    public sealed partial class ProfilePanel : PanelBase
    {
        public ProfilePanel()
        {
            this.InitializeComponent();
        }
        NavigationOption option = NavigationOption.Builder()
                .AddTransition(new ScaleTransition())
                .Build();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Host.Navigate(typeof(LoginPanel), option);
        }

        private void ComboBox_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    option = NavigationOption.Builder()
                        .AddTransition(new ScaleTransition())
                        .Build();
                    break;
                case 1:
                    option = NavigationOption.Builder()
                        .AddTransition(new SlideInTransition())
                        .Build();
                    break;
                case 2:
                    option = NavigationOption.Builder()
                        .AddTransition(new SlidePushTransition())
                        .Build();
                    break;
                case 3:
                    option = NavigationOption.Builder()
                        .AddTransition(new FadeInTransition())
                        .Build();
                    break;
                case 4:
                    option = NavigationOption.Builder()
                        .AddTransition(new ParallaxSlideTransition())
                        .Build();
                    break;
                case 5:
                    option = NavigationOption.Builder()
                        .AddTransition(new ScaleSlideTransition())
                        .Build();
                    break;
                case 6:
                    option = NavigationOption.Builder()
                        .AddTransition(new FadeInCompositionTransition(TimeSpan.FromSeconds(0.4)))
                        .Build();
                    break;
            }
        }
    }
}