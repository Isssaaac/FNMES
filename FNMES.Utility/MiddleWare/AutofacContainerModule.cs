#if !NETFRAMEWORK && !WINDOWS
using FNMES.Utility.Extension;

namespace FNMES.Utility.MiddleWare
{
    public class AutofacContainerModule
    {
        public static TService GetService<TService>() where TService : class
        {
            return MyHttpContext.ServiceProvider.GetService(typeof(TService)) as TService;
        }
    }
}
#endif
