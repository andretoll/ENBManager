using ENBManager.Infrastructure.BusinessEntities;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.SkyrimSE
{
    public class SkyrimModule : InstalledGame, IModule
    {
        #region InstalledGame Override

        public override string Title => "The Elder Scrolls V: Skyrim";
        public override string Executable => "Skyrim.exe";
        public override string InstalledLocation { get; set; }
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/skyrim.png"));

        #endregion

        #region Public Static Methods

        public static ModuleInfo GetModuleInfo()
        {
            return new ModuleInfo()
            {
                ModuleName = typeof(SkyrimModule).Name,
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
