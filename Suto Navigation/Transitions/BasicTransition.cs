using SutoNavigation.NavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace SutoNavigation.Transitions
{
    public class BasicTransition : PanelTransition
    {
        public BasicTransition()
        {
        }

        public override void SetInitialState(ref PanelBase userControl, bool isBack)
        {
        }

        public override List<Timeline> CreateAnimation(ref PanelBase userControl, bool isBack)
        {
            return new List<Timeline>();
        }
    }
}
