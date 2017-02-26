using Suto_Navigation.Sample.Panes;
using SutoNavigation;
using SutoNavigation.NavigationService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Suto_Navigation.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        PanelContainer panelContainer;

        public MainPage()
        {
            this.InitializeComponent();

            panelContainer = new PanelContainer();
            panelContainer.MinimumThresshold = 1;
            panelContainer.Navigate(typeof(ProfilePanel));
            //panelContainer.AnimationMode = AnimationMode.Composition;
            //Comment below line to use Nomarl mode, which create new Panel when navigating
            //panelContainer.OperationMode = OperationMode.Recycle;
            panelContainer.EnableAutoMemoryManagement(new BasicMemoryWatcher());
            root.Children.Add(panelContainer);


            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = this.panelContainer.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void Update_Panel_Count(object sender, RoutedEventArgs e)
        {
            StackCount_TB.Text = "Number of panel in stack: " + panelContainer.PanelStack.Count;
        }

        private void Fire_Low_Mem(object sender, RoutedEventArgs e)
        {
            panelContainer.RequestFreeMemory(new MemoryReportArgs(SutoNavigation.Interfaces.MemoryPressureStates.Medium));
        }
    }
}
