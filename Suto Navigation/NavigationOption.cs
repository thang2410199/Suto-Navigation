﻿using SutoNavigation.NavigationService;
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

        public Dictionary<string, object> Arguments { get; set; }

        public object GetArgument(string key)
        {
            if (Arguments != null && Arguments.ContainsKey(key))
                return Arguments[key];
            return null;
        }

        public NavigationOption SetArgument(string key, object value)
        {
            if (Arguments == null)
                Arguments = new Dictionary<string, object>();

            if (Arguments.ContainsKey(key))
                Arguments[key] = value;
            else
                Arguments.Add(key, value);
            return this;
        }

        public NavigationOption()
        {

        }

        public NavigationOption(PanelTransition transition, OperationMode operationMode, Dictionary<string, object> arguments)
        {
            this.Transition = transition;
            this.OperationMode = operationMode;
            this.Arguments = arguments;
        }

        public static NavigationOptionBuilder Builder()
        {
            return new NavigationOptionBuilder();
        }

        public class NavigationOptionBuilder
        {
            PanelTransition transition = new BasicTransition();

            OperationMode operationMode = OperationMode.Auto;

            Dictionary<string, object> arguments = null;

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

            public NavigationOptionBuilder AddArgument(Dictionary<string, object> arg)
            {
                this.arguments = arg;
                return this;
            }

            /// <summary>
            /// Return default option: BasicTranstion OperationMode.Auto
            /// </summary>
            public NavigationOption Build()
            {
                return new NavigationOption(this.transition, this.operationMode, this.arguments);
            }
        }
    }
}
