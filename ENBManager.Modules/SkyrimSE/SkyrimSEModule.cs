using Prism.Ioc;
using Prism.Modularity;
using System;

namespace ENBManager.Modules.SkyrimSE
{
    public class SkyrimSEModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new NotImplementedException();
        }

        public static ModuleInfo GetModuleInfo()
        {
            return new ModuleInfo()
            {
                ModuleName = nameof(SkyrimSEModule),
                ModuleType = typeof(SkyrimSEModule).ToString(),
                InitializationMode = InitializationMode.OnDemand
            };
        }
    }
}
