using Suto_Navigation_Composition.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Suto_Navigation_Composition;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Suto_Navigation.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Composition : Page, ISutoTransitionPage
    {
        public Composition()
        {
            this.InitializeComponent();
        }

        public TransitionFrame TransitionFrame
        {
            get
            {
                return this.Frame as TransitionFrame;
            }
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.TransitionFrame.TransitionTo(typeof(Composition2), null);
        }
    }
}
