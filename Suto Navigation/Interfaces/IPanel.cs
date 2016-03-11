using SutoNavigation.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutoNavigation.NavigationService.Interfaces
{
    public interface IPanel
    {
        IPanelHost Host { get; set; }
        PanelTransition Transition { get; set; }
        /// <summary>
        /// Fired when the panel is being created.
        /// </summary>
        /// <param name="host">A reference to the host.</param>
        /// <param name="arguments">Arguments for the panel</param>
        void PanelSetup(IPanelHost host, Dictionary<string, object> arguments);

        /// <summary>
        /// Fired when the panel is already in the stack, but a new navigate has been made to it.
        /// Instead of creating a new panel, this same panel is used and given the navigation arguments.
        /// </summary>
        /// <param name="arguments">The argumetns passed when navigate was called</param>
        void OnPanelPulledToTop(Dictionary<string, object> arguments);

        /// <summary>
        /// Fired before the panel is shown but when it is just about to be shown.
        /// </summary>
        void OnNavigatingTo();

        /// <summary>
        /// Fired just before the panel is going to be hidden.
        /// </summary>
        void OnNavigatingFrom();

        /// <summary>
        /// Fired when the panel is being removed from the navigation stack and will
        /// never be shown again.
        /// </summary>
        void OnCleanupPanel();

        /// <summary>
        /// Fired when the panel should try to reduce memory if possible. This will only be called
        /// while the panel isn't visible.
        /// </summary>
        void OnReduceMemory();

        /// <summary>
        /// Defined the likely that panel will be clear when memory in more pressure state.
        /// Low importance panel will be cleared first when memory start growing large.
        /// When memory is critical, Normal importance panels will be cleared.
        /// High importance panel will not be cleared under any situation
        /// </summary>
        Importaness Importaness { get; set; }
    }

    public enum Importaness
    {
        Low,
        Normal,
        High
    }
}
