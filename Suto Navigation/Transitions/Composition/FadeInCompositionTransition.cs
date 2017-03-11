using SutoNavigation.NavigationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using SutoNavigation.NavigationService.Interfaces;

namespace SutoNavigation.Transitions
{
    /// <summary>
    /// Animate Opacity of the Panel
    /// </summary>
    public class FadeInCompositionTransition : PanelTransition
    {
        float initialOpacity = 0;
        float endOpacity = 1;
        public FadeInCompositionTransition(TimeSpan duration, float initialOpacity = 0, float endOpacity = 1)
        {
            this.Duration = duration;
            this.initialOpacity = initialOpacity;
            this.endOpacity = endOpacity;
        }

        public FadeInCompositionTransition() : this(TimeSpan.FromMilliseconds(400))
        {
        }

        public override void Setup(ref PanelBase userControl, bool isGoBack)
        {
            base.Setup(ref userControl, isGoBack);
            if (!isGoBack)
            {
                userControl.Visual.Opacity = initialOpacity;
            }
        }

        public override List<(CompositionAnimationGroup animationGroup, IPanel panel)> CreateAnimationWithComposition(ref PanelBase panel, bool isBack)
        {
            var animation = panel.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0f, panel.Visual.Opacity);
            animation.InsertKeyFrame(1f, isBack ? this.initialOpacity : this.endOpacity);
            animation.Target = nameof(Visual.Opacity);
            animation.Duration = this.Duration;

            var animations = panel.Compositor.CreateAnimationGroup();
            animations.Add(animation);

            var result = new List<(CompositionAnimationGroup animationGroup, IPanel panel)>();
            result.Add((animations, panel));
            return result;
        }

        public override void Final(ref PanelBase currentPanel)
        {
            base.Final(ref currentPanel);

            currentPanel.Visual.Opacity = 1;
        }
    }
}
