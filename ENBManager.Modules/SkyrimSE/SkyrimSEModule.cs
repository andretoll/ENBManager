using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Views;
using Prism.Ioc;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.SkyrimSE
{
    public class SkyrimSEModule : GameModule
    {
        #region Constructor

        public SkyrimSEModule(IContainerProvider container)
            : base(container) { }

        #endregion

        #region GameModule Override

        public override string Title => "The Elder Scrolls V: Skyrim Special Edition";
        public override string Executable => "SkyrimSE.exe";
        public override string Module => ModuleNames.SKYRIMSE;
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/skyrimse.png"));
        public override string[] Binaries => new[] {"d3d11.dll", "d3dcompiler_46e.dll"};
        public override string Url => "https://www.nexusmods.com/skyrimspecialedition";

        public override void Activate()
        {
            ActivateModule(typeof(DashboardView), typeof(PresetsView), typeof(ScreenshotView));
        }

        #endregion
    }
}
