using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Skyrim
{
    [Module(ModuleName = ModuleNames.SKYRIM)]
    public class SkyrimModule : GameModule, IModule
    {
        #region GameModule Override

        public override string Title => "The Elder Scrolls V: Skyrim";
        public override string Executable => "Skyrim.exe";
        public override string Module => GetModuleInfo().ModuleName;
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/skyrim.png"));

        public override void Activate(IRegionManager regionManager)
        {
            base.Activate(regionManager);
        }

        #endregion

        #region Public Static Methods

        public static ModuleInfo GetModuleInfo()
        {
            return new ModuleInfo()
            {
                ModuleName = ModuleNames.SKYRIM,
                ModuleType = typeof(SkyrimModule).AssemblyQualifiedName,
                InitializationMode = InitializationMode.OnDemand
            };
        } 

        #endregion

        #region IModule Implementation

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        #endregion
    }
}
