using SutoNavigation.NavigationService;
using SutoNavigation.NavigationService.Interfaces;
using SutoNavigation.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public virtual void OnPanelPulledToTop(Dictionary<string, object> arguments)
        {
        }

        public virtual void OnReduceMemory()
        {
        }

        public virtual void PanelSetup(IPanelHost host, Dictionary<string, object> arguments)
        {
            this.Host = host;
        }
    }
}
