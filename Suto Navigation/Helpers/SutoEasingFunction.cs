using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace SutoNavigation.Helpers
{
    public class SutoEasingFunction
    {
        public static CubicBezierEasingFunction EaseIn(Compositor compositor)
        {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0.5f, 0), new Vector2(1, 1));
        }

        public static CubicBezierEasingFunction EaseOut(Compositor compositor) {
            return compositor.CreateCubicBezierEasingFunction(new Vector2(0, 0), new Vector2(0.5f, 1));
        }
    }
}
