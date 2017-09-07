﻿// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// THIS FILE IS NOT INTENDED TO BE EDITED. 
// 
// It has been imported using NuGet from the PocketContainer project (https://github.com/jonsequitur/PocketContainer). 
// 
// This file can be updated in-place using the Package Manager Console. To check for updates, run the following command:
// 
// PM> Get-Package -Updates

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pocket
{
    /// <summary>
    /// Performs runtime type discovery, useful for convention-based configuration tasks.
    /// </summary>
#if !SourceProject
    [System.Diagnostics.DebuggerStepThrough]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
    internal static class Discover
    {
        public static IEnumerable<Type> DerivedFrom(this IEnumerable<Type> types, Type type) =>
            types.Where(type.IsAssignableFrom);

        public static IEnumerable<Type> ConcreteTypes() =>
            Types()
                .Where(t => !t.IsAbstract &&
                            !t.IsInterface &&
                            !t.IsGenericTypeDefinition);

        public static IEnumerable<Type> ImplementingOpenGenericInterfaces(
            this IEnumerable<Type> source,
            params Type[] interfaces) =>
            source.Where(t => t.GetInterfaces()
                               .Any(i => i.IsConstructedGenericType &&
                                         interfaces.Contains(i.GetGenericTypeDefinition())));

        private static IEnumerable<Type> Types() =>
            AppDomain.CurrentDomain
                     .GetAssemblies()
                     .Where(a => !a.IsDynamic)
                     .Where(a => !a.GlobalAssemblyCache)
                     .SelectMany(a =>
                     {
                         try
                         {
                             return a.DefinedTypes;
                         }
                         catch (TypeLoadException)
                         {
                         }
                         catch (ReflectionTypeLoadException)
                         {
                         }
                         catch (FileNotFoundException)
                         {
                         }
                         catch (FileLoadException)
                         {
                         }
                         return Enumerable.Empty<Type>();
                     });
    }
}