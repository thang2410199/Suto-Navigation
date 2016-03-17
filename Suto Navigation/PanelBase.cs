using SutoNavigation.Interfaces;
using SutoNavigation.NavigationService.Interfaces;
using SutoNavigation.Transitions;
using Windows.UI.Xaml.Controls;

namespace SutoNavigation.NavigationService
{
    public class PanelBase : UserControl, IPanel
    {
        public IPanelHost Host { get; set; }

        /// <summary>
        /// Default Importaness is Normal
        /// </summary>
        public Importaness Importaness { get; set; } = Importaness.Normal;

        /// <summary>
        /// Transition data use when navigate to and from. Default behaviour is try to revert the transition
        /// if is assigned to null value, no transition will play
        /// </summary>
        public PanelTransition Transition
        {
            get; set;
        } = new BasicTransition();

        public virtual void OnCleanupPanel()
        {
            
        }

        public virtual void OnNavigatingFrom()
        {
        }

        public virtual void OnNavigatingTo()
        {
        }

        public virtual void OnPanelPulledToTop(NavigationOption options)
        {
        }

        public virtual void OnReduceMemory()
        {
        }

        public virtual void PanelSetup(IPanelHost host, NavigationOption options)
        {
            this.Host = host;
        }
    }
}
