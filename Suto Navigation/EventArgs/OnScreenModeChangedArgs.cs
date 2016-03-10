using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutoNavigation.NavigationService
{
    public class OnScreenModeChangedArgs : EventArgs
    {
        public ScreenMode NewScreenMode;
    }

    public enum ScreenMode
    {
        Split,
        Single,
        FullScreen
    }
}
