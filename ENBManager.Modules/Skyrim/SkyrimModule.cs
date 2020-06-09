using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Views;
using Prism.Ioc;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Skyrim
{
    public class SkyrimModule : GameModule
    {
        #region Constructor

        public SkyrimModule(IContainerProvider container)
            : base(container) { }

        #endregion

        #region GameModule Override

        public override string Title => "The Elder Scrolls V: Skyrim";
        public override string Executable => "Skyrim.exe";
        public override string Module => ModuleNames.SKYRIM;
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/skyrim.png"));
        public override string[] Binaries => new[] { "d3d11.dll", "d3dcompiler_46e.dll" };

        public override void Activate()
        {
            ActivateModule(typeof(DashboardView));
        }

        #endregion
    }
}
