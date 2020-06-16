using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Services;
using ENBManager.Core.Helpers;
using ENBManager.Core.ViewModels;
using ENBManager.Core.Views;
using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Interfaces;
using ENBManager.Infrastructure.Services;
using ENBManager.Logging.Services;
using ENBManager.Modules.Fallout4;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Services;
using ENBManager.Modules.Shared.ViewModels;
using ENBManager.Modules.Shared.Views;
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
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

        private static Mutex _mutex;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion

        #region Overriden Methods

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;

            _mutex = new Mutex(true, Assembly.GetExecutingAssembly().GetName().Name, out createdNew);

            if (!createdNew)
            {
                Process current = Process.GetCurrentProcess();
                foreach (var process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        RestoreWindow(process);
                    }
                }

                Current.Shutdown();
            }

            base.OnStartup(e);
        }

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
            _ = containerRegistry.RegisterSingleton<IScreenshotWatcher, ScreenshotWatcher>();
            _ = containerRegistry.Register<ILoggerFacade, PrismLogger>();
            _ = containerRegistry.Register<IGameService, GameService>();
            _ = containerRegistry.Register<ISnackbarMessageQueue, SnackbarMessageQueue>();
            _ = containerRegistry.Register<IPresetManager, PresetManager>();
            _ = containerRegistry.Register<IScreenshotManager, ScreenshotManager>();

            // ViewModels
            _ = containerRegistry.RegisterSingleton<DashboardViewModel>();
            _ = containerRegistry.RegisterSingleton<PresetsViewModel>();
            _ = containerRegistry.RegisterSingleton<ScreenshotViewModel>();

            // Dialogs
            containerRegistry.RegisterDialog<DiscoverGamesDialog, DiscoverGamesDialogViewModel>();
            containerRegistry.RegisterDialog<AppSettingsDialog, AppSettingsViewModel>();
            containerRegistry.RegisterDialog<GameSettingsDialog, GameSettingsViewModel>();
            containerRegistry.RegisterDialog<AddPresetDialog, AddPresetDialogViewModel>();
            containerRegistry.RegisterDialog<AboutDialog, AboutViewModel>();
        }

        #endregion

        #region Private Methods

        private void ConfigureGameModuleCatalog()
        {
            _logger.Debug("Configuring game modules");

            var catalog = Container.Resolve<IGameModuleCatalog>();

            catalog.Register<Fallout4Module>(Container);
            catalog.Register<SkyrimSEModule>(Container);
            catalog.Register<SkyrimModule>(Container);
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
            _logger.Debug("Applying theme");

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

        private void RestoreWindow(Process process)
        {
            IntPtr handle = process.MainWindowHandle;

            // If window is minimized to tray
            if (handle == IntPtr.Zero)
            {
                handle = FindWindow(null, ApplicationName.NAME);
            }

            // If window is minimized
            if (IsIconic(handle))
            {
                ShowWindow(handle, 9);
            }

            SetForegroundWindow(handle);
        }

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
