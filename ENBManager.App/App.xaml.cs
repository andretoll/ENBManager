﻿using ENBManager.Core.Helpers;
using ENBManager.Core.Interfaces;
using ENBManager.Core.Services;
using ENBManager.Core.ViewModels;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Logging.Services;
using ENBManager.Modules.Fallout4;
using ENBManager.Modules.Skyrim;
using ENBManager.Modules.SkyrimSE;
using NLog;
using NLog.Config;
using NLog.Targets;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
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

        private static Logger _logger = LogManager.GetCurrentClassLogger(); 

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

            _logger.Info($"Starting {Assembly.GetExecutingAssembly().GetName().Name} v{Assembly.GetExecutingAssembly().GetName().Version}");

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
            _ = containerRegistry.Register<ILoggerFacade, PrismLogger>();
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

        #endregion

        #region Private Methods

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

            switch (logLevel)
            {
                case Logging.Enums.LogLevel.Debug:
                    return LogLevel.Debug;
                case Logging.Enums.LogLevel.Information:
                    return LogLevel.Info;
                case Logging.Enums.LogLevel.Error:
                    return LogLevel.Error;
                default:
                    throw new ArgumentException("Unsupported log level.");
            }
        }

        #endregion
    }
}
