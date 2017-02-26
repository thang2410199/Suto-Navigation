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
using SutoNavigation.Interfaces;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SutoNavigation.NavigationService
{
    public sealed partial class PanelContainer : UserControl, IPanelHost, IMemoryReactor
    {
        public AnimationMode AnimationMode { get; set; } = AnimationMode.Transformer;
        public int MinimumThresshold { get; set; } = 0;

        bool _AutoMemoryManagementEnabled = false;
        public bool AutoMemoryManagementEnabled
        {
            get
            {
                return _AutoMemoryManagementEnabled;
            }
        }

        public static Compositor Compositor = new Compositor();

        IMemoryWatcher memoryWatcher;
        IMemoryReactor memoryReactor;

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
        private CompositionAnimationGroup CompositionAnimationGroup;
        private CompositionScopedBatch AnimationBatch;

        public PanelContainer()
        {
            this.InitializeComponent();

            PanelStack = new List<PanelBase>();
        }

        public void EnableAutoMemoryManagement(IMemoryWatcher memWatcher)
        {
            _AutoMemoryManagementEnabled = true;
            memoryWatcher = memWatcher;
            memoryReactor = this;
            memoryWatcher.OnMemoryNeeded += memoryReactor.OnMemoryNeeded;
            memoryWatcher.StartWatch();
        }

        public void EnableAutoMemoryManagement(IMemoryWatcher memWatcher, IMemoryReactor reactor)
        {
            _AutoMemoryManagementEnabled = true;
            memoryWatcher = memWatcher;
            memoryReactor = reactor;
            memoryWatcher.OnMemoryNeeded += memoryReactor.OnMemoryNeeded;
            memoryWatcher.StartWatch();
        }

        void IMemoryReactor.OnMemoryNeeded(object sender, MemoryReportArgs e)
        {
            //return if Low or None as we dont need to do anything
            if (e.CurrentPressure == (MemoryPressureStates.Low | MemoryPressureStates.None))
                return;
            List<PanelBase> panelToClear = null;
            Importaness target = Importaness.Low;
            //react to current memory state
            switch (e.CurrentPressure)
            {
                case MemoryPressureStates.Medium:
                    target = Importaness.Low;
                    break;
                case MemoryPressureStates.High:
                    target = Importaness.Low | Importaness.Normal;
                    break;
            }

            //Never clear current pannel
            //panelToClear = PanelStack.Take(PanelStack.Count - 1).Where(p => p.Importaness == target).ToList();
            //var size = panelToClear.Count;
            //if (size > 0)
            //{
            //    for (int j = 0; j < size; j++)
            //    {
            //        var panel = panelToClear[j];
            //        FireOnReduceMemory(panel);
            //        panel.Transition.Cleanup(ref panel);

            //        //Remove from stack
            //        PanelStack.Remove(panel);

            //        //Remove from visual
            //        root.Children.Remove(panel);
            //    }
            //}

            ////Re apply effect for the remained panel, from top to bottm
            ////TODO: Find a more optimized way
            //for (int k = PanelStack.Count - 1; k > 0; k--)
            //{
            //    var lastPanel = PanelStack[k - 1];
            //    PanelStack[k].Transition.SetupPreviousPanel(ref lastPanel);
            //}

            PanelBase previousPanel = PanelStack.Count > MinimumThresshold ? PanelStack[MinimumThresshold - 1] : null;
            int i = MinimumThresshold;
            bool needResetVisual = false;
            // From bottom to top, e.g: 0 -> 4
            while (i < PanelStack.Count)
            {
                if (PanelStack[i].Importaness == target && i != (PanelStack.Count - 1))
                {
                    var panel = PanelStack[i];
                    FireOnReduceMemory(panel);

                    //Remove from stack
                    PanelStack.Remove(panel);

                    //Remove from visual
                    root.Children.Remove(panel);

                    needResetVisual = true;
                }
                else
                {
                    if (previousPanel != null && needResetVisual == true)
                    {
                        //once the low importance panel is removed, we need to set the previous panel state to comfort this panel transition
                        PanelStack[i].Transition.SetupPreviousPanel(ref previousPanel);
                        needResetVisual = false;
                    }

                    previousPanel = PanelStack[i];

                    i++;
                }
            }
        }

        public void DisableAutoMemoryManagement()
        {
            _AutoMemoryManagementEnabled = false;
            memoryWatcher.OnMemoryNeeded -= memoryReactor.OnMemoryNeeded;
            memoryWatcher.StopWatch();
        }

        public void RequestFreeMemory(MemoryReportArgs e)
        {
            memoryWatcher.FireMemoryNeeded(e);
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

                // Give it a chance to react
                var handled = leavingPanel.OnGoBack();
                if (!handled)
                    return false;

                //// Remove the panel, use the index or we will remove the wrong one!
                //PanelStack.RemoveAt(PanelStack.Count - 1);

                if (PanelStack.Count > 2)
                {
                    PanelStack[PanelStack.Count - 2].Visibility = Visibility.Visible;
                    PanelStack[PanelStack.Count - 3].Visibility = Visibility.Visible;
                }
                else
                {
                    if (PanelStack.Count > 0)
                    {
                        PanelStack[0].Visibility = Visibility.Visible;
                    }
                }
                FireOnNavigateFrom(leavingPanel);
                if (leavingPanel.Transition.GetType() != typeof(BasicTransition))
                {
                    // TODO: Save animation state when navigating to to use here, or allow custom transition from outside
                    //transitionStoryboard?.Stop();
                    SetupTransition(ref leavingPanel, true);
                    if (transitionStoryboard != null && this.AnimationMode == AnimationMode.Transformer)
                    {
                        transitionStoryboard.Begin();
                    }
                    else
                    {
                        var visual = ElementCompositionPreview.GetElementVisual(leavingPanel);
                        visual.StartAnimationGroup(CompositionAnimationGroup);
                        AnimationBatch.End();
                    }
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

        public bool Navigate(Type panelType, NavigationOption options = null)
        {
            //Fire NavigatedFrom event for current panel
            if (PanelStack.Count > 0)
            {
                var currentPanel = PanelStack.LastOrDefault();
                FireOnNavigateFrom(currentPanel);
            }

            if (options == null)
                options = NavigationOption.Builder().Build();
            PanelBase panel = null;

            //if not set OperationMode, use global setting
            var operationMode = OperationMode;
            if (options.OperationMode != OperationMode.Auto)
                operationMode = options.OperationMode;

            switch (operationMode)
            {
                case OperationMode.Normal:
                    // if type is normal create new instance of panel
                    panel = (PanelBase)Activator.CreateInstance(panelType);
                    break;
                case OperationMode.Recycle:

                    //Find existing panel
                    var oldPanel = this.PanelStack.FirstOrDefault(p => p.GetType() == panelType);
                    if (oldPanel != null)
                    {
                        //Reset the animation applied to realated panel
                        oldPanel.Transition.ResetPreviousView(ref oldPanel);

                        var oldPanelIndex = PanelStack.IndexOf(oldPanel);
                        // If its not the first Panel, re apply next panel transition initial state to the previous panel
                        if (oldPanelIndex > 0 && oldPanelIndex + 1 < PanelStack.Count)
                        {
                            var previousPanel = PanelStack[oldPanelIndex - 1];
                            PanelStack[oldPanelIndex + 1].Transition.SetupPreviousPanel(ref previousPanel);
                        }

                        //Move oldPanel to the last in Stack.
                        PanelStack.Remove(oldPanel);

                        //Remove it from Grid to avoid duplication as it will be added later
                        root.Children.Remove(oldPanel);

                        panel = oldPanel;
                    }
                    else
                    {
                        // if panel is null then create new one
                        panel = (PanelBase)Activator.CreateInstance(panelType);
                    }
                    break;
                default:
                    panel = (PanelBase)Activator.CreateInstance(panelType);
                    break;
            }

            if (options.Transition != null)
                panel.Transition = options.Transition;
            PanelStack.Add(panel);
            // Give data to panel to start getting / processing ahead of animation to create illusion of speed :)
            panel.PanelSetup(this, options);
            // Use transition to add the panel to visual tree
            SetUpPanelAsControl(ref panel);
            return true;
        }

        private void SetUpPanelAsControl(ref PanelBase panel)
        {
            // Assign a render transform to panel if not have yet
            var transform = panel.RenderTransform as CompositeTransform;
            if (transform == null)
            {
                transform = new CompositeTransform();
                panel.RenderTransform = transform;
            }

            //transitionStoryboard?.Stop();
            // Setup animation for transition only if custom transition implemented
            if (panel.Transition.GetType() != typeof(BasicTransition))
                SetupTransition(ref panel);

            this.root.Children.Add(panel);

            // Hide unuse panel to save render power and avoid getting in the way of more complex transition 
            if (PanelStack.Count >= 3)
            {
                PanelStack[PanelStack.Count - 3].Visibility = Visibility.Collapsed;
            }

            if (panel.Transition.GetType() == typeof(BasicTransition) ||
                (this.AnimationMode == AnimationMode.Transformer && this.transitionStoryboard.Children.Count == 0) ||
                (this.AnimationMode == AnimationMode.Composition && this.CompositionAnimationGroup.Count == 0)
                )
            {
                PanelAnimation_Completed(null, null);
            }
            else
            {
                if (this.AnimationMode == AnimationMode.Transformer)
                {
                    transitionStoryboard.Begin();
                }
                else
                {
                    var visual = ElementCompositionPreview.GetElementVisual(panel);
                    visual.StartAnimationGroup(CompositionAnimationGroup);
                    AnimationBatch.End();
                }
                state = NavigationState.Transiting;
            }
        }

        private void SetupTransition(ref PanelBase panel, bool isBack = false)
        {
            switch (AnimationMode)
            {
                case AnimationMode.Transformer:
                    SetupTransformerTransition(ref panel, isBack);
                    break;
                case AnimationMode.Composition:
                    SetupCompositionTransition(ref panel, isBack);
                    break;
            }
        }

        private void SetupTransformerTransition(ref PanelBase panel, bool isBack = false)
        {
            transitionStoryboard = new Storyboard();
            //We need to reset because the panel can be reused, not just created.
            panel.Transition.ResetView(ref panel, isBack);
            panel.Transition.Setup(ref panel, isBack);
            var animations = panel.Transition.CreateAnimation(ref panel, isBack);
            if (animations.Count == 0)
            {
                return;
            }

            foreach (var item in animations)
            {
                transitionStoryboard.Children.Add(item);
            }

            if (isBack)
                transitionStoryboard.Completed += PanelBackAnimation_Completed;
            else
                transitionStoryboard.Completed += PanelAnimation_Completed;
        }

        private void SetupCompositionTransition(ref PanelBase panel, bool isBack = false)
        {
            panel.Transition.ResetView(ref panel, isBack);
            panel.Transition.Setup(ref panel, isBack);



            var visual = ElementCompositionPreview.GetElementVisual(panel);
            this.AnimationBatch = visual.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            var group = visual.Compositor.CreateAnimationGroup();
            var animations = panel.Transition.CreateAnimationWithComposition(ref panel, isBack);

            if (animations.Count == 0)
            {
                AnimationBatch.End();
                return;
            }
            foreach (var item in animations)
            {
                group.Add(item);
            }
            this.CompositionAnimationGroup = group;

            if (isBack)
                AnimationBatch.Completed += PanelBackAnimation_Completed;
            else
                AnimationBatch.Completed += PanelAnimation_Completed;
        }


        public Task<double> SetStatusBar(Color? color = default(Color?), double opacity = 1)
        {
            throw new NotImplementedException();
        }

        public void ToggleFullScreen(bool goFullScreen)
        {
            throw new NotImplementedException();
        }

        private void PanelAnimation_Completed(object sender, object e)
        {
            // Update the back button
            UpdateBackButton();
            var lastPanel = PanelStack.Last();
            lastPanel.Transition.Final(ref lastPanel);
            FireOnNavigateTo(lastPanel);
            state = NavigationState.Idle;

            FireOnNavigateComplete();
        }

        private void PanelBackAnimation_Completed(object sender, object e)
        {
            var leavingPanel = PanelStack.Last();

            FireOnCleanupPanel(leavingPanel);
            PanelStack.RemoveAt(PanelStack.Count - 1);
            root.Children.Remove(leavingPanel);
            if (PanelStack.Count > 0)
            {
                FireOnNavigateTo(PanelStack.LastOrDefault());
            }
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
        Auto,
        Normal,
        Recycle,
    }

    public enum AnimationMode
    {
        Transformer,
        Composition
    }
}
