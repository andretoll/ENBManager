using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models;
using ENBManager.Configuration.Services;
using ENBManager.Core.Helpers;
using ENBManager.Core.Interfaces;
using ENBManager.Core.Services;
using ENBManager.Core.ViewModels;
using ENBManager.Core.Views;
using ENBManager.Modules.SkyrimSE;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Services.Dialogs;
using Prism.Unity;
using System.Windows;

namespace ENBManager.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void InitializeShell(Window shell)
        {
            ApplyTheme();

            RunDiscoverGames();

            base.InitializeShell(shell);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _ = containerRegistry.RegisterSingleton<IConfigurationManager<AppSettings>, ConfigurationManager<AppSettings>>();
            _ = containerRegistry.Register<IGameLocator, GameLocator>();
            _ = containerRegistry.Register<IFileService, FileService>();
            containerRegistry.RegisterDialog<DiscoverGamesDialog, DiscoverGamesDialogViewModel>();
            containerRegistry.RegisterDialog<AppSettingsDialog, AppSettingsViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(SkyrimModule.GetModuleInfo());
            moduleCatalog.AddModule(SkyrimSEModule.GetModuleInfo());
            moduleCatalog.AddModule(Fallout4Module.GetModuleInfo());
        }

        private void ApplyTheme()
        {
            var configurationManager = Container.Resolve<IConfigurationManager<AppSettings>>();

            ThemeHelper.UpdateTheme(configurationManager.Settings.DarkMode);
        }

        private void RunDiscoverGames()
        {
            var manager = Container.Resolve<IConfigurationManager<AppSettings>>();

            // If app has not yet initialized, run the discover games tool
            if (!manager.Settings.Initialized)
            {
                Container.Resolve<IDialogService>().ShowDialog(nameof(DiscoverGamesDialog), new DialogParameters(), (dr) =>
                {
                    if (dr.Result != ButtonResult.OK)
                        Current.Shutdown();
                    else
                    {
                        manager.Settings.Initialized = true;
                        manager.SaveSettings();
                    }
                });
            }
        }
    }
}
