using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.SkyrimSE
{
    [Module(ModuleName = ModuleNames.SKYRIMSE)]
    public class SkyrimSEModule : InstalledGame, IModule
    {
        #region InstalledGame Override

        public override string Title => "The Elder Scrolls V: Skyrim Special Edition";
        public override string Executable => "SkyrimSE.exe";
        public override string Module => GetModuleInfo().ModuleName;
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/skyrimse.png"));

        public override void Activate(IRegionManager regionManager)
        {
            base.Activate(regionManager);
            
            regionManager.RequestNavigate(RegionNames.MainRegion, nameof(ModuleShell));

            regionManager.AddToRegion(RegionNames.TabRegion, new PresetsView(this));
            regionManager.AddToRegion(RegionNames.TabRegion, new SettingsView(this));
        }

        #endregion

        #region Public Static Methods

        public static ModuleInfo GetModuleInfo()
        {
            return new ModuleInfo()
            {
                ModuleName = ModuleNames.SKYRIMSE,
                ModuleType = typeof(SkyrimSEModule).AssemblyQualifiedName,
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
