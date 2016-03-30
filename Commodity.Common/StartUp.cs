using System;
using System.Collections.Generic;
using System.Linq;
using Commodity.Interfaces;
using Ninject;
using Ninject.Infrastructure.Language;

namespace Commodity.Common
{
    public static class ReflectionTypeExtensions
    {
        public static IEnumerable<Type> FindTypesImplementingInterface<TInterface>(
            this System.Reflection.Assembly[] assemblies)
        {
            return assemblies.FindTypesImplementingInterface(typeof (TInterface));
        }
        public static IEnumerable<Type> FindTypesImplementingInterface(this System.Reflection.Assembly[] assemblies, Type typeToFind)
        {
            var allTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                if (typeToFind.IsGenericType)
                {
                    allTypes.AddRange(assembly.GetTypes().Where(t => t.GetInterfaces().Any(i=> i.IsGenericType && typeToFind.IsAssignableFrom(i.GetGenericTypeDefinition()))));
                }
                else
                {
                    allTypes.AddRange(assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeToFind)));
                }
            }
            return allTypes.ToEnumerable();
        }
    } 

    public static class StartUpKernelExtension
    {
        public static void BindStartUp<TStartUpAs>(this IKernel kernel) where TStartUpAs : IStartUp
        {
            // Find all TStartUpAs's objects in all assemblies
            var allStartUpTypes = AppDomain.CurrentDomain.GetAssemblies().FindTypesImplementingInterface<TStartUpAs>();
            foreach (Type t in allStartUpTypes)
            {
                kernel.Bind<TStartUpAs>().To(t);
            }
        }
    }
}
