using ENBManager.Configuration.Interfaces;
using ENBManager.Configuration.Models;
using ENBManager.Configuration.Services;
using ENBManager.Core.Views;
using ENBManager.Modules.SkyrimSE;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System.Windows;

namespace ENBManager.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _ = containerRegistry.Register<IConfigurationManager<AppSettings>, ConfigurationManager<AppSettings>>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(SkyrimSEModule.GetModuleInfo());
        }
    }
}
