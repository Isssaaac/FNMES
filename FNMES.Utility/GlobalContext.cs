using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FNMES.Utility.Core;

namespace FNMES.Utility
{
    public static class GlobalContext
    {
        public static readonly IEnumerable<Assembly> Assemblies;
        public static readonly IEnumerable<Type> EffectiveTypes;
        public static SystemConfig SystemConfig { get; set; }
        static GlobalContext() 
        {
            Assemblies = GetAssemblies();
            EffectiveTypes = Assemblies.SelectMany(GetTypes);
        }
        public static IEnumerable<Assembly> GetAssemblies()
        {
            //加载所有程序集，包括project和dll
            var projects1 = DependencyContext
                                .Default
                                .RuntimeLibraries
                                .Where(u => u.Type == "project" || u.Type == "reference")
                                .Select(u => Assembly.Load(u.Name));
            var projects = AssemblyHelper.GetAssemblies();
            return projects;
        }
        private static IEnumerable<Type> GetTypes(Assembly ass)
        {
            var types = Array.Empty<Type>();
            try
            {
                types = ass.GetTypes();
            }
            catch
            {
                Console.WriteLine($"Error load '{ass.FullName}' assembly.");
            }
            return types.Where(u => u.IsPublic);
        }
    }
}
