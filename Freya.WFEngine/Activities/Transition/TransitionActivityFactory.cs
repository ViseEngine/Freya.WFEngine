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
    public class TransitionActivityFactory : IXmlComponentFactory<IActivity>
    {
        public IActivity CreateComponent(Type activityType, System.Xml.XmlElement configuration)
        {
            if (activityType == typeof(TransitionActivity))
                return new TransitionActivity(configuration.GetAttribute(SR.DefaultExitStateAttributeName));

            if (activityType == typeof(AutoTransitionActivity))
                return new AutoTransitionActivity(configuration.GetAttribute(SR.DefaultExitStateAttributeName));

            throw new NotSupportedException();
        }

        public bool CanHandle(Type activityType) {
            return activityType == typeof (TransitionActivity) || activityType == typeof(AutoTransitionActivity);
        }
    }
}
