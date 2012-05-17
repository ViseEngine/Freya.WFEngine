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
using System.Xml;

namespace Freya.WFEngine.TestApp
{
    public class AskGuardFactory<TItem> : IXmlComponentFactory<IGuard<TItem>>
    {
        public bool CanHandle(Type componentType) {
            return componentType == typeof (AskGuard<TItem>);
        }

        public IGuard<TItem> CreateComponent(Type componentType, XmlElement configuration) {
            return new AskGuard<TItem>(configuration.GetAttribute("question"));
        }
    }
}
