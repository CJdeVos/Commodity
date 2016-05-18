using System;
using Commodity.Interfaces;
using Ninject;

namespace Commodity.Common
{
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
