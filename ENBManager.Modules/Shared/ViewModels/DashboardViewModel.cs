using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Events;
using ENBManager.Modules.Shared.Models;
using ENBManager.Modules.Shared.ViewModels.Base;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using System;
using System.IO;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class DashboardViewModel : TabItemBase
    {
        #region Private Members

        private readonly IEventAggregator _eventAggregator;

        #endregion

        public DashboardViewModel(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public override string Name => Strings.DASHBOARD;

        protected override void OnModuleActivated(GameModule game)
        {
            //TODO: Verify installation path
            if (!File.Exists(Path.Combine(game.Settings.InstalledLocation, game.Executable)))
            {
                _eventAggregator.GetEvent<ShowSnackbarMessageEvent>().Publish("TEST");
            }

            //TODO: Verify binaries
        }
    }
}
