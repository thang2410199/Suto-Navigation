using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SutoNavigation.Transitions
{
    public class TransitionKeyPairValue
    {
        public DependencyProperty Property { get; set; }
        public object Value { get; set; }

        public TransitionKeyPairValue(DependencyProperty dp, object value)
        {
            Property = dp;
            Value = value;
        }
    }
}
