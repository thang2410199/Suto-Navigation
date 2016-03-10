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
using SutoNavigation.Transitions;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SutoNavigation.NavigationService
{
    public sealed partial class PanelContainer : UserControl, IPanelHost
    {
        public int MinimumThresshold { get; set; } = 0;

        public Size Size
        {
            get
            {
                return this.RenderSize;
            }
        }

        Storyboard transitionStoryboard;

        /// <summary>
        /// Allow Container to work in recycle mode.
        /// Default is Nomarl, container create new Panel for each navigation request
        /// Recycle Mode: TODO, try to reuse current available Panel when navigating, possible change navigation stack order
        /// </summary>
        public OperationMode OperationMode { get; set; } = OperationMode.Normal;

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


        public List<PanelBase> PanelStack { get; set; }
        private NavigationState state;

        public PanelContainer()
        {
            this.InitializeComponent();

            PanelStack = new List<PanelBase>();
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
            PanelBase leavingPanel = null;
            lock (PanelStack)
            {
                //TODO: Handle if transition is running
                if (state != NavigationState.Idle)
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

                if (leavingPanel.Transition != null)
                {
                    // TODO: Save animation state when navigating to to use here, or allow custom transition from outside
                    //transitionStoryboard?.Stop();
                    SetupTransition(ref leavingPanel, true);
                    transitionStoryboard.Begin();
                    state = NavigationState.Transiting;
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
            var panel = (PanelBase)Activator.CreateInstance(panelType);
            panel.Transition = transition;
            panel.PanelSetup(this, arguments);
            SetUpPanelAsControl(ref panel);
            return true;
        }

        private void SetUpPanelAsControl(ref PanelBase panel)
        {
            var transform = panel.RenderTransform as CompositeTransform;
            if (transform == null)
            {
                transform = new CompositeTransform();
                panel.RenderTransform = transform;
            }

            //transitionStoryboard?.Stop();
            if (panel.Transition != null)
                SetupTransition(ref panel);
            PanelStack.Add(panel);

            this.root.Children.Add(panel);
            if (panel.Transition != null)
            {
                transitionStoryboard.Begin();
                state = NavigationState.Transiting;
            }
            else
            {
                FireOnNavigateComplete();
            }
            UpdateBackButton();
        }

        private void SetupTransition(ref PanelBase panel, bool isBack = false)
        {
            transitionStoryboard = new Storyboard();

            panel.Transition.SetInitialState(ref panel, isBack);
            var animations = panel.Transition.CreateAnimation(ref panel, isBack);
            foreach (var item in animations)
            {
                transitionStoryboard.Children.Add(item);
            }


            if (isBack)
                transitionStoryboard.Completed += PanelBackAnimation_Completed;
            else
                transitionStoryboard.Completed += PanelAnimation_Completed;

            return;

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
            state = NavigationState.Idle;
            FireOnNavigateComplete();
        }

        private void PanelBackAnimation_Completed(object sender, object e)
        {
            var leavingPanel = PanelStack.Last();
            FireOnNavigateFrom(leavingPanel);
            FireOnCleanupPanel(leavingPanel);
            PanelStack.RemoveAt(PanelStack.Count - 1);
            root.Children.Remove(leavingPanel);

            UpdateBackButton();
            state = NavigationState.Idle;
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

    public enum NavigationState
    {
        Idle,
        FadingIn,
        FadingOut,
        Transiting,
    }

    public enum OperationMode
    {
        Normal,
        Recycle,
    }
}
