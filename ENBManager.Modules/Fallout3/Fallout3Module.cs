using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Views;
using Prism.Ioc;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Fallout4
{
    public class Fallout3Module : GameModule
    {
        #region Constructor

        public Fallout3Module(IContainerProvider container)
            : base(container) { }

        #endregion

        #region GameModule Override

        public override string Title => "Fallout 3";
        public override string Executable => "Fallout3.exe";
        public override string Module => ModuleNames.FALLOUT3;
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/fallout3.png"));
        public override string[] Binaries => new[] { "d3d9.dll" };
        public override string Url => "https://www.nexusmods.com/fallout3";

        public override void Activate()
        {
            ActivateModule(typeof(DashboardView), typeof(PresetsView), typeof(ScreenshotView));
        }

        #endregion
    }
}
