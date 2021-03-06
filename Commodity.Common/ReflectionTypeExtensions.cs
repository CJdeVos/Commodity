using System;
using System.Collections.Generic;
using System.Linq;
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
            return allTypes;
        }

        public static IEnumerable<Type> FindTypesWithAttribute<TAttributeType>(this System.Reflection.Assembly[] assemblies) where TAttributeType: Attribute
        {
            var allTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                allTypes.AddRange(assembly.GetTypes().Where(t => t.HasAttribute<TAttributeType>()));
            }
            return allTypes;
        }
    }
}