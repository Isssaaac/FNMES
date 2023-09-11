#if !NETFRAMEWORK
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FNMES.Utility.Extension
{
    public static class MyHttpContext
    {
        public static IServiceProvider ServiceProvider;
        static MyHttpContext()
        { }

        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));
                Microsoft.AspNetCore.Http.HttpContext context = ((HttpContextAccessor)factory).HttpContext;
                return context;
            }
        }

        public static HttpContext httpContext
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));
                HttpContext context = ((HttpContextAccessor)factory).HttpContext;
                return context;
            }
        }


        public static IHostingEnvironment HostingEnvironment
        {
            get
            {
                return ServiceProvider.GetRequiredService<IHostingEnvironment>();
            }
        }

        public static string MapPath(string path)
        {
            return HostingEnvironment.ContentRootPath.Replace('\\', '/') + path;
        }
        public static string WebRootPath(string path)
        {
            return HostingEnvironment.WebRootPath.Replace('\\', '/') + path;
        }

        //public static IHttpContextAccessor HttpContextAccessor
        //{
        //    get
        //    {
        //        return (IHttpContextAccessor)ServiceProvider.GetService(typeof(IHttpContextAccessor));
        //    }
        //}
    }
}
#endif