using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Core.Helpers;
using ENBManager.Core.Interfaces;
using ENBManager.Core.Services;
using ENBManager.Core.ViewModels;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Logging.Services;
using ENBManager.Modules.Fallout4;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Services;
using ENBManager.Modules.Shared.ViewModels;
using ENBManager.Modules.Skyrim;
using ENBManager.Modules.SkyrimSE;
using MaterialDesignThemes.Wpf;
using NLog;
using NLog.Config;
using NLog.Targets;
using Prism.Ioc;
using Prism.Logging;
using Prism.Services.Dialogs;
using Prism.Unity;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace ENBManager.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Overriden Methods

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.Info($"Exiting {Assembly.GetExecutingAssembly().GetName().Name}");

            base.OnExit(e);
        }

        protected override void InitializeShell(Window shell)
        {
            ConfigureLogging();

            ConfigureGameModuleCatalog();

            ApplyTheme();

            RunDiscoverGames();

            base.InitializeShell(shell);
            
            _logger.Info($"Starting {Assembly.GetExecutingAssembly().GetName().Name} v{Assembly.GetExecutingAssembly().GetName().Version}");
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Services
            _ = containerRegistry.RegisterSingleton<IConfigurationManager<AppSettings>, ConfigurationManager<AppSettings>>();
            _ = containerRegistry.RegisterSingleton<IGameModuleCatalog, GameModuleCatalog>();
            _ = containerRegistry.Register<ILoggerFacade, PrismLogger>();
            _ = containerRegistry.Register<IGameLocator, GameLocator>();
            _ = containerRegistry.Register<IFileService, FileService>();
            _ = containerRegistry.Register<ISnackbarMessageQueue, SnackbarMessageQueue>();
            _ = containerRegistry.Register<IPresetManager, PresetManager>();

            // ViewModels
            _ = containerRegistry.RegisterSingleton<DashboardViewModel>();
            _ = containerRegistry.RegisterSingleton<PresetsViewModel>();
            _ = containerRegistry.RegisterSingleton<SettingsViewModel>();

            // Dialogs
            containerRegistry.RegisterDialog<DiscoverGamesDialog, DiscoverGamesDialogViewModel>();
            containerRegistry.RegisterDialog<AppSettingsDialog, AppSettingsViewModel>();
        }

        #endregion

        #region Private Methods

        private void ConfigureGameModuleCatalog()
        {
            var catalog = Container.Resolve<IGameModuleCatalog>();

            catalog.AddModule<Fallout4Module>(Container);
            catalog.AddModule<SkyrimSEModule>(Container);
            catalog.AddModule<SkyrimModule>(Container);
        }

        private void ConfigureLogging()
        {
            string loggingPath = Path.Combine(Paths.GetBaseDirectory(), $"{Assembly.GetExecutingAssembly().GetName().Name}.log");
            string archivePath = Path.Combine(Paths.GetBaseDirectory(), "archives", $"{Assembly.GetExecutingAssembly().GetName().Name}.log");

            var config = new LoggingConfiguration();

            var logfile = new FileTarget("logfile")
            {
                FileName = loggingPath,
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}",
                ArchiveFileName = archivePath,
                ArchiveAboveSize = 100000000,
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence
            };
            config.AddTarget(logfile);
            config.AddRule(GetLogLevel(), LogLevel.Fatal, logfile);
            LogManager.Configuration = config;

            DispatcherUnhandledException += (sender, e) => { _logger.Error(e.Exception, "Unhandled exception"); };
        }

        private void ApplyTheme()
        {
            var configurationManager = Container.Resolve<IConfigurationManager<AppSettings>>();

            ThemeHelper.UpdateTheme(configurationManager.Settings.DarkMode);
            ThemeHelper.UpdateColorScheme(configurationManager.Settings.ColorScheme);
        }

        private void RunDiscoverGames()
        {
            var manager = Container.Resolve<IConfigurationManager<AppSettings>>();

            // If app has not yet initialized, run the discover games tool
            if (!manager.Settings.Initialized)
            {
                _logger.Info("Running Games Discovery for first time");
                Container.Resolve<IDialogService>().ShowDialog(nameof(DiscoverGamesDialog), new DialogParameters(), (dr) =>
                {
                    if (dr.Result != ButtonResult.OK)
                    {
                        Current.Shutdown();
                    }
                    else
                    {
                        manager.Settings.Initialized = true;
                        manager.SaveSettings();
                    }

                    _logger.Debug(nameof(RunDiscoverGames) + " - " + dr.Result);
                });
            }
        }

        #endregion

        #region Helper Methods

        private LogLevel GetLogLevel()
        {
            var logLevel = Container.Resolve<ConfigurationManager<AppSettings>>().Settings.LogLevel;

            return logLevel switch
            {
                Logging.Enums.LogLevel.Debug => LogLevel.Debug,
                Logging.Enums.LogLevel.Information => LogLevel.Info,
                Logging.Enums.LogLevel.Error => LogLevel.Error,
                _ => throw new ArgumentException("Unsupported log level."),
            };
        }

        #endregion
    }
}
