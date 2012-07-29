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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    public class CompositeComponentFactory<TComponent> : IComponentFactory<TComponent>
    {
        #region Helper class
        public class FactoryList : IList<IComponentFactory<TComponent>>
        {
            private readonly CompositeComponentFactory<TComponent> owner;

            public FactoryList(CompositeComponentFactory<TComponent> owner) {
                if (owner == null)
                    throw new ArgumentNullException("owner");

                this.owner = owner;
            }
            
            private List<IComponentFactory<TComponent>> list = new List<IComponentFactory<TComponent>>();
 
            public IEnumerator<IComponentFactory<TComponent>> GetEnumerator() {
                return list.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public void Add(IComponentFactory<TComponent> item) {
                if (item == null)
                    throw new ArgumentNullException("item");
                
                if (object.ReferenceEquals(owner, item))
                    throw new ArgumentException("Cannot add self to the collection.");

                list.Add(item);
            }

            public void AddRange(IEnumerable<IComponentFactory<TComponent>> factories) {
                if (factories == null)
                    throw new ArgumentNullException("factories");
                
                foreach (var factory in factories) {
                    this.Add(factory);
                }
            }

            public void Clear() {
                list.Clear();
            }

            public bool Contains(IComponentFactory<TComponent> item) {
                return list.Contains(item);
            }

            public void CopyTo(IComponentFactory<TComponent>[] array, int arrayIndex) {
                list.CopyTo(array, arrayIndex);
            }

            public bool Remove(IComponentFactory<TComponent> item) {
                return list.Remove(item);
            }

            public int Count {
                get { return list.Count; }
            }

            public bool IsReadOnly {
                get { return false; }
            }

            public int IndexOf(IComponentFactory<TComponent> item) {
                return list.IndexOf(item);
            }

            public void Insert(int index, IComponentFactory<TComponent> item) {
                if (item == null)
                    throw new ArgumentNullException("item");
                
                list.Insert(index, item);
            }

            public void RemoveAt(int index) {
                list.RemoveAt(index);
            }

            public IComponentFactory<TComponent> this[int index] {
                get { return list[index]; }
                set {
                    if (value == null)
                        throw new ArgumentNullException("value");

                    list[index] = value;
                }
            }
        }
        #endregion

        private readonly FactoryList factories;

        public CompositeComponentFactory() {
            this.factories = new FactoryList(this);
        } 

        public FactoryList Factories {
            get { return this.factories; }
        }

        [Obsolete("Use Factories property instead.")]
        public void Register(IComponentFactory<TComponent> componentFactory) {
            this.Factories.Add(componentFactory);
        }

        [Obsolete("Use Factories property instead.")]
        public void RegisterAll(IEnumerable<IComponentFactory<TComponent>> factoriesEnumeration) {
            this.Factories.AddRange(factoriesEnumeration);
        }

        public bool CanHandle(Type activityType) {
            return this.factories.Any(f => f.CanHandle(activityType));
        }

        public TComponent CreateComponent(Type componentType, IDictionary<string, object> parameters) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");
            
            for (int index = this.factories.Count - 1; index >= 0; index--) {
                IComponentFactory<TComponent> factory = this.factories[index];
                if (factory.CanHandle(componentType))
                    return factory.CreateComponent(componentType, parameters);
            }

            throw new NotSupportedException("There is no registered factory for type " + componentType);
        }
    }
}
