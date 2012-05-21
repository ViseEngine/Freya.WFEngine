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

namespace Freya.WFEngine
{
    public class CompositeComponentFactory<TComponent> : IComponentFactory<TComponent>
    {
        private readonly List<IComponentFactory<TComponent>> factories = new List<IComponentFactory<TComponent>>();

        public void Register(IComponentFactory<TComponent> componentFactory) {
            if (componentFactory == null)
                throw new ArgumentNullException("componentFactory");

            this.factories.Add(componentFactory);
        }

        public void RegisterAll(IEnumerable<IComponentFactory<TComponent>> factoriesEnumeration) {
            this.factories.AddRange(factoriesEnumeration);
        }

        public bool CanHandle(Type activityType) {
            return this.factories.Any(f => f.CanHandle(activityType));
        }

        public TComponent CreateComponent(Type componentType, IDictionary<string, object> parameters) {
            IComponentFactory<TComponent> factory = this.factories.First(f => f.CanHandle(componentType));
            return factory.CreateComponent(componentType, parameters);
        }
    }
}
