#if !NETFRAMEWORK
using Autofac;
using FNMES.Utility.Extension.SqlSugar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using ServiceStack.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Utility.MiddleWare
{
    public static class AutofacContainerModuleExtension
    {
        public static IServiceCollection AddModule(this IServiceCollection services, ContainerBuilder builder, IConfiguration configuration)
        {
            var compilationLibrary = DependencyContext.Default.RuntimeLibraries.Where(x => !x.Serviceable && x.Type == "project").ToList();
            List<Assembly> assemblyList = new List<Assembly>();

            foreach (var _compilation in compilationLibrary)
            {
                try
                {
                    assemblyList.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(_compilation.Name)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(_compilation.Name + ex.Message);
                }
            }

            foreach (var assembly in assemblyList)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var serviceAttribute = type.GetCustomAttribute<AppServiceAttribute>();
                    if (serviceAttribute is not null)
                    {
                        //情况1：使用自定义[AppService(ServiceType = typeof(注册抽象或者接口))]，手动去注册，放type即可
                        var serviceType = serviceAttribute.ServiceType;
                        //情况2 自动去找接口，如果存在就是接口，如果不存在就是本身
                        if (serviceType == null)
                        {
                            //获取最靠近的接口
                            var firstInter = type.GetInterfaces().LastOrDefault();
                            if (firstInter is null)
                            {
                                serviceType = type;
                            }
                            else
                            {
                                serviceType = firstInter;
                            }
                        }

                        switch (serviceAttribute.ServiceLifetime)
                        {
                            case LifeTime.Singleton:
                                if (type.IsGenericType)
                                {
                                    builder.RegisterGeneric(type).As(serviceType).SingleInstance();
                                }
                                else
                                {
                                    builder.RegisterType(type).As(serviceType).SingleInstance();
                                }
                                break;
                            case LifeTime.Scoped:
                                if (type.IsGenericType)
                                {
                                    builder.RegisterGeneric(type).As(serviceType).InstancePerLifetimeScope();
                                }
                                else
                                {
                                    builder.RegisterType(type).As(serviceType).InstancePerLifetimeScope();
                                }
                                break;
                            case LifeTime.Transient:
                                if (type.IsGenericType)
                                {
                                    builder.RegisterGeneric(type).As(serviceType).InstancePerDependency();
                                }
                                else
                                {
                                    builder.RegisterType(type).As(serviceType).InstancePerDependency();
                                }
                                break;
                            default:
                                if (type.IsGenericType)
                                {
                                    builder.RegisterGeneric(type).As(serviceType).InstancePerDependency();
                                }
                                else
                                {
                                    builder.RegisterType(type).As(serviceType).InstancePerDependency();
                                }
                                break;
                        }
                    }
                }
            }
            return services;
        }
    }
}
#endif