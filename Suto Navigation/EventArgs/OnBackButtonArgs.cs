using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutoNavigation.NavigationService
{
    /// <summary>
    /// Provides data for BaconManager's OnBackButton event.
    /// </summary>
    public class OnBackButtonArgs : EventArgs
    {
        /// <summary>
        /// If the back button press has been handled already.
        /// </summary>
        public bool IsHandled = false;
    }
}
