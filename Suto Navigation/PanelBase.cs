using SutoNavigation.NavigationService;
using SutoNavigation.NavigationService.Interfaces;
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
        protected IPanelHost host;

        public PanelTransition Transition
        {
            get; set;
        } = new PanelTransition(PanelTransitionType.None);

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
            this.host = host;
        }
    }
}
