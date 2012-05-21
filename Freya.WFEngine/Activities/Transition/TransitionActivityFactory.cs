#region License
// 
// Author: Lukas Paluzga <sajagi@freya.cz>
// Copyright (c) 2012, Lukas Paluzga
//  
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//  
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public class TransitionActivityFactory : IComponentFactory<IActivity>
    {
        public IActivity CreateComponent(Type activityType, IDictionary<string, object> parameters) {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            string exitState = (string) parameters[SR.DefaultExitStateAttributeName];
            if (exitState == null)
                throw new ArgumentException("Parameters must contain a string value for " + SR.DefaultExitStateAttributeName + " key.");

            if (activityType == typeof(TransitionActivity))
                return new TransitionActivity(exitState);

            if (activityType == typeof(AutoTransitionActivity))
                return new AutoTransitionActivity(exitState);

            throw new NotSupportedException("Activity of type " + activityType + " is not supported.");
        }

        public bool CanHandle(Type activityType) {
            return activityType == typeof (TransitionActivity) || activityType == typeof(AutoTransitionActivity);
        }
    }
}
