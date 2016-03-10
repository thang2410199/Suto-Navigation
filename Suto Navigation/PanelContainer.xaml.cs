using SutoNavigation.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using SutoNavigation.NavigationService.Interfaces;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SutoNavigation.NavigationService
{
    public sealed partial class PanelContainer : UserControl, IPanelHost
    {
        public int MinimumThresshold { get; set; } = 0;

        public double FeedbackOffset { get; set; } = 100;

        Storyboard transitionStoryboard;

        /// <summary>
        /// Fired when the screen mode changes
        /// </summary>
        public event EventHandler<OnScreenModeChangedArgs> OnScreenModeChanged
        {
            add { onScreenModeChanged.Add(value); }
            remove { onScreenModeChanged.Remove(value); }
        }
        SmartWeakEvent<EventHandler<OnScreenModeChangedArgs>> onScreenModeChanged = new SmartWeakEvent<EventHandler<OnScreenModeChangedArgs>>();

        /// <summary>
        /// Fired when the navigation is complete
        /// </summary>
        public event EventHandler<EventArgs> OnNavigationComplete
        {
            add { onNavigationComplete.Add(value); }
            remove { onNavigationComplete.Remove(value); }
        }
        SmartWeakEvent<EventHandler<EventArgs>> onNavigationComplete = new SmartWeakEvent<EventHandler<EventArgs>>();


        List<IPanel> PanelStack;
        private State state;

        public PanelContainer()
        {
            this.InitializeComponent();

            PanelStack = new List<IPanel>();
        }

        public ScreenMode CurrentScreenMode()
        {
            throw new NotImplementedException();
        }

        public bool GoBack()
        {
            bool handled = false;

            // If we can go back, do it.
            if (CanGoBack())
            {
                // Call go back but this might not work, we can't go back while something else is navigating.
                // If we can't go back right now just silently ignore the request.
                bool? wentBack = GoBack_Internal();

                // If null was returned we are animating a navigation so just ignore this.
                if (!wentBack.HasValue)
                {
                    handled = true;
                }
                // If we got true then we actually did go back.
                else if (wentBack.Value)
                {
                    handled = true;
                }
                // If we get false we can't go back and din't go anything.
                else
                {
                    handled = false;
                }
            }

            if (!handled)
            {
                //// If we can't go back anymore for the last back show the menu.
                //// After that let the user leave.
                //if (!m_finalNavigateHasShownMenu)
                //{
                //    m_finalNavigateHasShownMenu = true;
                //    handled = true;
                //    ToggleMenu(true);
                //}
            }

            return handled;
        }

        private bool? GoBack_Internal()
        {
            IPanel leavingPanel = null;
            lock (PanelStack)
            {
                //TODO: Handle if transition is running
                if (state != State.Idle)
                {
                    // We can't do anything if we are already animating.
                    return null;
                }

                if (PanelStack.Count <= 0)
                {
                    // We can't go back, there is nothing to go back to.
                    return false;
                }

                // Get the panel we are removing.
                leavingPanel = PanelStack.Last();
                //// Remove the panel, use the index or we will remove the wrong one!
                //PanelStack.RemoveAt(PanelStack.Count - 1);

                if (leavingPanel.Transition.Type != PanelTransitionType.None)
                {
                    // TODO: Save animation state when navigating to to use here, or allow custom transition from outside
                    transitionStoryboard?.Stop();
                    SetupTransition(ref leavingPanel, true);
                    transitionStoryboard.Begin();
                    state = State.Transiting;
                }
                else
                {
                    // While not under lock inform the panel it is leaving.
                    // If Transition is none, just leave now
                    PanelBackAnimation_Completed(null, null);
                }
            }

            return true;
        }

        public bool Navigate(Type panelType, Dictionary<string, object> arguments = null, PanelTransition transition = null)
        {
            var panel = (IPanel)Activator.CreateInstance(panelType);
            panel.PanelSetup(this, arguments);
            if (transition != null)
                panel.Transition = transition;
            SetUpPanelAsControl(ref panel);
            return true;
        }

        private void SetUpPanelAsControl(ref IPanel panel)
        {

            transitionStoryboard?.Stop();
            SetupTransition(ref panel);
            PanelStack.Add(panel);

            this.root.Children.Add((UserControl)panel);
            if (panel.Transition.Type != PanelTransitionType.None)
            {
                transitionStoryboard.Begin();
                state = State.Transiting;
            }
            else
            {
                FireOnNavigateComplete();
            }
            UpdateBackButton();
        }
        UserControl currentPanel;
        UserControl lastPanel;
        private void SetupTransition(ref IPanel panel, bool isBack = false)
        {
            currentPanel = (UserControl)panel;
            var transform = currentPanel.RenderTransform as CompositeTransform;

            if (transform == null)
            {
                transform = new CompositeTransform();
                currentPanel.RenderTransform = transform;
            }

            transitionStoryboard = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation();

            animation.Duration = panel.Transition.Duration;
            if (panel.Transition.EasingFunction != null)
                animation.EasingFunction = panel.Transition.EasingFunction;
            else
                animation.EasingFunction = isBack ? new QuadraticEase() { EasingMode = EasingMode.EaseOut } : new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            string propertyName = "";
            double newPosition = 0;
            switch (panel.Transition.Type)
            {
                case PanelTransitionType.FadeIn:
                    currentPanel.Opacity = isBack ? 1 : 0;
                    animation.From = currentPanel.Opacity;
                    animation.To = isBack ? 0 : 1;
                    Storyboard.SetTarget(animation, currentPanel);
                    Storyboard.SetTargetProperty(animation, "Opacity");
                    break;
                case PanelTransitionType.SlideIn:
                case PanelTransitionType.BounceIn:
                    propertyName = GetTransitionProperty(panel.Transition.Direction);

                    newPosition = GetSlideTransitionProperty(panel.Transition.Direction);
                    //animation.From = isBack ? 0 : newPosition;
                    animation.To = !isBack ? 0 : newPosition;
                    Storyboard.SetTarget(animation, transform);
                    Storyboard.SetTargetProperty(animation, propertyName);
                    break;
                case PanelTransitionType.InstagramLike:
                    propertyName = GetTransitionProperty(TransitionDirection.RightToLeft);
                    newPosition = GetSlideTransitionProperty(TransitionDirection.RightToLeft);
                    animation.From = isBack ? transform.TranslateX : newPosition;
                    animation.To = !isBack ? 0 : newPosition;
                    Storyboard.SetTarget(animation, transform);
                    Storyboard.SetTargetProperty(animation, propertyName);


                    break;

                default:
                    break;
            }
            transitionStoryboard.Children.Add(animation);

            if (panel.Transition.Type == PanelTransitionType.InstagramLike)
            {
                if (PanelStack.Count > 0)
                {
                    lastPanel = (isBack ? PanelStack[PanelStack.Count - 2] : PanelStack.Last()) as UserControl;
                    var lastTransform = lastPanel.RenderTransform as CompositeTransform;

                    DoubleAnimation lastAnimation = new DoubleAnimation();
                    //lastAnimation.From = isBack ? -500 :0;
                    lastAnimation.To = isBack ? 0 : -FeedbackOffset;
                    lastAnimation.Duration = panel.Transition.Duration;
                    lastAnimation.EasingFunction = !isBack ? new QuadraticEase() { EasingMode = EasingMode.EaseOut } : new QuadraticEase() { EasingMode = EasingMode.EaseIn };
                    Storyboard.SetTarget(lastAnimation, lastTransform);
                    Storyboard.SetTargetProperty(lastAnimation, propertyName);
                    transitionStoryboard.Children.Add(lastAnimation);
                }
                
                if (isBack)
                    UnregisterManipulation(ref currentPanel);
                else
                    RegisterManipulation(ref currentPanel);            
            }

            if (isBack)
                transitionStoryboard.Completed += PanelBackAnimation_Completed;
            else
                transitionStoryboard.Completed += PanelAnimation_Completed;

            return;

        }

        private void RegisterManipulation(ref UserControl userControl)
        {
            userControl.ManipulationDelta += UserControl_ManipulationDelta;
            userControl.ManipulationCompleted += UserControl_ManipulationCompleted;
        }

        private void UnregisterManipulation(ref UserControl userControl)
        {
            userControl.ManipulationDelta -= UserControl_ManipulationDelta;
            userControl.ManipulationCompleted -= UserControl_ManipulationCompleted;
        }

        private void UserControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var transform = currentPanel.RenderTransform as CompositeTransform;
            if (transform.TranslateX < FeedbackOffset)
            {
                //Move it back to 0

                DoubleAnimation animation = new DoubleAnimation();
                animation.To = 0;
                animation.Duration = TimeSpan.FromMilliseconds(200);
                QuadraticEase ease = new QuadraticEase();
                ease.EasingMode = EasingMode.EaseOut;
                animation.EasingFunction = ease;

                Storyboard storyboard = new Storyboard();
                Storyboard.SetTarget(animation, transform);
                Storyboard.SetTargetProperty(animation, "TranslateX");
                storyboard.Children.Add(animation);
                storyboard.Begin();

            }
            else
            {
                //Invoke back event.
                GoBack();
            }
        }

        private void UserControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var transform = currentPanel.RenderTransform as CompositeTransform;
            var newX = transform.TranslateX;
            newX += e.Delta.Translation.X;
            if (newX < 0)
                newX = 0;
            else
            {
                var lastTransform = lastPanel.RenderTransform as CompositeTransform;

                lastTransform.TranslateX = (1 - newX / this.RenderSize.Width) * -FeedbackOffset;
            }

            transform.TranslateX = newX;
        }

        private double GetSlideTransitionProperty(TransitionDirection direction)
        {
            switch (direction)
            {
                case TransitionDirection.BottomToTop:
                    return this.RenderSize.Height;
                case TransitionDirection.TopToBottom:
                    return -this.RenderSize.Height;
                case TransitionDirection.LeftToRight:
                    return -this.RenderSize.Width;
                case TransitionDirection.RightToLeft:
                    return this.RenderSize.Width;
            }
            return this.RenderSize.Width;
        }

        private string GetTransitionProperty(TransitionDirection direction)
        {
            switch (direction)
            {
                case TransitionDirection.BottomToTop:
                case TransitionDirection.TopToBottom:
                    return "TranslateY";
                case TransitionDirection.LeftToRight:
                case TransitionDirection.RightToLeft:
                    return "TranslateX";
            }
            return "TranslateX";
        }

        public Task<double> SetStatusBar(Color? color = default(Color?), double opacity = 1)
        {
            throw new NotImplementedException();
        }

        public void ToggleFullScreen(bool goFullScreen)
        {
            throw new NotImplementedException();
        }

        public void ToggleMenu(bool show)
        {
            throw new NotImplementedException();
        }

        private void PanelAnimation_Completed(object sender, object e)
        {
            // Update the back button
            UpdateBackButton();
            FireOnNavigateTo(PanelStack.Last());
            state = State.Idle;
            FireOnNavigateComplete();
        }

        private void PanelBackAnimation_Completed(object sender, object e)
        {
            var leavingPanel = PanelStack.Last();
            FireOnNavigateFrom(leavingPanel);
            FireOnCleanupPanel(leavingPanel);
            PanelStack.RemoveAt(PanelStack.Count - 1);

            currentPanel = PanelStack.Last() as UserControl;
            root.Children.Remove((UserControl)leavingPanel);

            UpdateBackButton();
            state = State.Idle;
        }

        #region handle back request
        /// <summary>
        /// Updates the current state of the back button
        /// </summary>
        private void UpdateBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = CanGoBack() ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private bool CanGoBack()
        {
            return PanelStack.Count > MinimumThresshold;
        }

        #endregion

        #region fire event

        /// <summary>
        /// Fires OnNavigationComplete
        /// </summary>
        /// <param name="panel"></param>
        private void FireOnNavigateComplete()
        {
            try
            {
                onNavigationComplete.Raise(this, new EventArgs());
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Fires OnNavigateFrom for the panel
        /// </summary>
        /// <param name="panel"></param>
        private void FireOnNavigateFrom(IPanel panel)
        {
            try
            {
                // Tell the panel it is leaving
                panel.OnNavigatingFrom();
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Fires OnNavigateTo for the panel
        /// </summary>
        /// <param name="panel"></param>
        private void FireOnNavigateTo(IPanel panel)
        {
            try
            {
                // Tell the panel it is prime time!
                panel.OnNavigatingTo();
            }
            catch (Exception e)
            {

            }
        }


        /// <summary>
        /// Fires OnCleanupPanel for the panel
        /// </summary>
        /// <param name="panel"></param>
        private void FireOnCleanupPanel(IPanel panel)
        {
            try
            {
                // Tell the panel to clean up.
                panel.OnCleanupPanel();
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Fires OnReduceMemory for the panel
        /// </summary>
        /// <param name="panel"></param>
        private void FireOnReduceMemory(IPanel panel)
        {
            try
            {
                // Tell the panel to reduce memory.
                panel.OnReduceMemory();
            }
            catch (Exception e)
            {

            }
        }

        #endregion
    }
}
