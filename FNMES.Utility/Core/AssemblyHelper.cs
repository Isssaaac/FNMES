using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.Core
{
    public static class AssemblyHelper
    {
        public static IEnumerable<Assembly> GetAssemblies()
        {
            return GetAssemblies(null);
        }

        public static IEnumerable<Assembly> GetAssemblies(Func<RuntimeLibrary, bool> filter)
        {
            var dependencyContext = DependencyContext.Default;

            if (dependencyContext == null)
            {
                // 回退方案：返回当前域中已加载的程序集
                return AppDomain.CurrentDomain.GetAssemblies()
                    .Where(asm => !asm.IsDynamic && !string.IsNullOrEmpty(asm.Location));
            }

            // 默认过滤器：包含项目程序集和引用程序集
            var defaultFilter = new Func<RuntimeLibrary, bool>(lib =>
                lib.Type == "project" || lib.Type == "reference" || lib.Type == "package");

            var finalFilter = filter ?? defaultFilter;

            var assemblies = new List<Assembly>();
            var loadedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var library in dependencyContext.RuntimeLibraries.Where(finalFilter))
            {
                try
                {
                    // 避免重复加载
                    if (loadedAssemblies.Contains(library.Name))
                        continue;

                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                    loadedAssemblies.Add(library.Name);
                }
                catch (FileNotFoundException)
                {
                    // 程序集可能不存在，跳过
                    Console.WriteLine($"程序集 {library.Name} 未找到");
                }
                catch (BadImageFormatException)
                {
                    // 无效的程序集文件，跳过
                    Console.WriteLine($"程序集 {library.Name} 格式无效");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"加载程序集 {library.Name} 时出错: {ex.Message}");
                }
            }

            return assemblies;
        }
    }
}
