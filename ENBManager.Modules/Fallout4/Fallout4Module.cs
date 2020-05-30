using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Views;
using Prism.Ioc;
using System;
using System.Windows.Media.Imaging;

namespace ENBManager.Modules.Fallout4
{
    public class Fallout4Module : GameModule
    {
        #region Constructor

        public Fallout4Module(IContainerProvider container)
            : base(container) { }

        #endregion

        #region GameModule Override

        public override string Title => "Fallout 4";
        public override string Executable => "Fallout4.exe";
        public override string Module => ModuleNames.FALLOUT4;
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Infrastructure;component/Resources/Icons/fallout4.png"));
        public override string[] Binaries => throw new NotImplementedException();

        public override void Activate()
        {
            ActivateModule(typeof(DashboardView), typeof(PresetsView), typeof(SettingsView));
        }

        #endregion
    }
}
