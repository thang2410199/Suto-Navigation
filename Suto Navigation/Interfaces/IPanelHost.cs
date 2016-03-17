using SutoNavigation.NavigationService;
using SutoNavigation.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace SutoNavigation.Interfaces
{
    public interface IPanelHost : IMemoryManager
    {
        Size Size { get; }
        /// <summary>
        /// Thresshold for back button visibility, default value should be 0, used in <see cref="GoBack"/> method
        /// </summary>
        int MinimumThresshold { get; set; }


        /// <summary>
        /// Store all panel in the navigation stack
        /// </summary>
        List<PanelBase> PanelStack { get; set; }

        /// <summary>
        /// Fire when navigation is complete
        /// </summary>
        event EventHandler<EventArgs> OnNavigationComplete;

        /// <summary>
        /// Navigates to a panel. If a panel already exist with the same panelId instead or creating a new
        /// panel the old panel will be shown and passed the new arguments.
        /// </summary>
        /// <param name="panelType">The type of panel to be created</param>
        /// <param name="panelId">A unique identifier for the panel, the id should be able to differeincae between two panels of the same type</param>
        /// <param name="arguments">Arguments to be sent to the panel</param>
        /// <returns></returns>
        bool Navigate(Type panelType, NavigationOption options = null);

        /// <summary>
        /// Called to navigate back to the last panel.
        /// </summary>
        bool GoBack();
    }
}
