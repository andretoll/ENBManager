using ENBManager.Infrastructure.BusinessEntities;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.SkyrimSE
{
    public class Fallout4Module : InstalledGame, IModule
    {
        #region InstalledGame Override

        public override string Title => "Fallout 4";
        public override string Executable => "Fallout4.exe";
        public override string InstalledLocation { get; set; }
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/fallout4.png")); 

        #endregion

        #region Public Static Methods

        public static ModuleInfo GetModuleInfo()
        {
            return new ModuleInfo()
            {
                ModuleName = typeof(Fallout4Module).Name,
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
