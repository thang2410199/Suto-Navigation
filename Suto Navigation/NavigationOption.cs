using SutoNavigation.NavigationService;
using SutoNavigation.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutoNavigation
{
    public class NavigationOption
    {
        public PanelTransition Transition { get; set; }

        public OperationMode OperationMode { get; set; }

        public NavigationOption()
        {
            
        }

        public NavigationOption(PanelTransition transition, OperationMode operationMode)
        {
            this.Transition = transition;
            this.OperationMode = operationMode;
        }

        public static NavigationOptionBuilder Builder()
        {
            return new NavigationOptionBuilder();
        }

        public class NavigationOptionBuilder
        {
            PanelTransition transition = new BasicTransition();

            OperationMode operationMode = OperationMode.Auto;

            public NavigationOptionBuilder AddTransition(PanelTransition transition)
            {
                this.transition = transition;
                return this;
            }

            public NavigationOptionBuilder AddOperationMode(OperationMode mode)
            {
                this.operationMode = mode;
                return this;
            }

            /// <summary>
            /// Return default option: BasicTranstion OperationMode.Auto
            /// </summary>
            public NavigationOption Build()
            {
                return new NavigationOption(this.transition, this.operationMode);
            }
        }
    }
}
