using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Fallout4
{
    [Module(ModuleName = ModuleNames.FALLOUT4)]
    public class Fallout4Module : InstalledGame, IModule
    {
        #region InstalledGame Override

        public override string Title => "Fallout 4";
        public override string Executable => "Fallout4.exe";
        public override string Module => GetModuleInfo().ModuleName;
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/fallout4.png"));

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
                ModuleName = ModuleNames.FALLOUT4,
                ModuleType = typeof(Fallout4Module).AssemblyQualifiedName,
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
