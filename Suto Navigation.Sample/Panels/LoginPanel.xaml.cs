using SutoNavigation;
using SutoNavigation.NavigationService;
using SutoNavigation.Transitions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Suto_Navigation.Sample.Panes
{
    public sealed partial class LoginPanel : PanelBase
    {
        List<SolidColorBrush> datas = new List<SolidColorBrush>();
        float _animationDuration = 1000;
        public LoginPanel()
        {
            this.InitializeComponent();

            var random = new Random();
            for (int i = 0; i < 500; i++)
            {
                datas.Add(new SolidColorBrush(Color.FromArgb((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255))));
            }

            this.gridView.ItemsSource = datas;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Host.Navigate(typeof(ProviderPanel),
                NavigationOption.Builder()
                .AddTransition(new ParallaxSlideTransition())
                .Build());
        }

        private void gridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {

        }

        private void gridView_ChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args)
        {
            //if (!args.IsContainerPrepared)
            //{
            //    return;
            //}
            args.ItemContainer = args.ItemContainer ?? new GridViewItem();

            var fadeIn = this.Compositor.CreateScalarKeyFrameAnimation();
            fadeIn.Target = "Opacity";
            fadeIn.Duration = TimeSpan.FromMilliseconds(_animationDuration);
            fadeIn.InsertKeyFrame(0, 0);
            fadeIn.InsertKeyFrame(1, 1);
            //fadeIn.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            if (!args.IsContainerPrepared)
            {
                fadeIn.DelayTime = TimeSpan.FromMilliseconds(_animationDuration * 0.125 * args.ItemIndex);
            }

            var fadeOut = this.Compositor.CreateScalarKeyFrameAnimation();
            fadeOut.Target = "Opacity";
            fadeOut.Duration = TimeSpan.FromMilliseconds(_animationDuration);
            fadeOut.InsertKeyFrame(1, 0);

            var scaleIn = this.Compositor.CreateVector3KeyFrameAnimation();
            scaleIn.Target = "Scale";
            scaleIn.Duration = TimeSpan.FromMilliseconds(_animationDuration);
            scaleIn.InsertKeyFrame(0f, new Vector3(1.2f, 1.2f, 1.2f));
            scaleIn.InsertKeyFrame(1f, new Vector3(1f, 1f, 1f));
            //scaleIn.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            if (!args.IsContainerPrepared)
            {
                scaleIn.DelayTime = TimeSpan.FromMilliseconds(_animationDuration * 0.125 * args.ItemIndex);
            }

            // animations set to run at the same time are grouped
            var animationFadeInGroup = this.Compositor.CreateAnimationGroup();
            animationFadeInGroup.Add(fadeIn);
            animationFadeInGroup.Add(scaleIn);

            var visual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
            visual.StartAnimationGroup(animationFadeInGroup);
            Debug.WriteLine("Started animation at index " + args.ItemIndex);

            // Set up show and hide animations for this item container before the element is added to the tree.
            // These fire when items are added/removed from the visual tree, including when you set Visibilty
            //ElementCompositionPreview.SetImplicitShowAnimation(args.ItemContainer, animationFadeInGroup);
            //ElementCompositionPreview.SetImplicitHideAnimation(args.ItemContainer, fadeOut);

            args.ItemContainer.Tag = true;
        }
    }
}
