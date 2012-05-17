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
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    public class ActivityProxyGenerationHook : IProxyGenerationHook
    {
        private static readonly ActivityProxyGenerationHook defaultInstance = new ActivityProxyGenerationHook();

        private ActivityProxyGenerationHook() {
        }

        public static ActivityProxyGenerationHook DefaultInstance {
            get { return defaultInstance; }
        }

        public void MethodsInspected() {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo) {
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo) {
            return Attribute.IsDefined(methodInfo, typeof (InvocationMethodAttribute));
        }
    }
}
