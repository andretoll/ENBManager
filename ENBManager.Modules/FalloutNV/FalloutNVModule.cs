using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Views;
using Prism.Ioc;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Fallout4
{
    public class FalloutNVModule : GameModule
    {
        #region Constructor

        public FalloutNVModule(IContainerProvider container)
            : base(container) { }

        #endregion

        #region GameModule Override

        public override string Title => "Fallout: New Vegas";
        public override string Executable => "FalloutNV.exe";
        public override string Module => ModuleNames.FALLOUTNV;
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/falloutnv.png"));
        public override string[] Binaries => new[] { "d3d9.dll" };
        public override string Url => "https://www.nexusmods.com/newvegas";

        public override void Activate()
        {
            ActivateModule(typeof(DashboardView), typeof(PresetsView), typeof(ScreenshotView));
        }

        #endregion
    }
}
