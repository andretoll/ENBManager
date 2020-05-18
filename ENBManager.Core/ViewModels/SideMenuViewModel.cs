using ENBManager.Core.Helpers;
using ENBManager.Core.Interfaces;
using ENBManager.Infrastructure.BusinessEntities;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ENBManager.Core.ViewModels
{
    public class SideMenuViewModel : BindableBase
    {
        #region Private Members

        private readonly IFileService _fileService;
        private readonly IModuleCatalog _moduleCatalog;
        private readonly IModuleManager _moduleManager;

        private InstalledGame _selectedGame;

        #endregion

        #region Public Properties

        public ObservableCollection<InstalledGame> Games { get; set; }
        public InstalledGame SelectedGame
        {
            get { return _selectedGame; }
            set
            {
                _selectedGame = value;
                RaisePropertyChanged();

                ActivateModule(value.Module);
            }
        }

        #endregion

        #region Commands

        public DelegateCommand GetDataCommand { get; set; }

        #endregion

        #region Constructor

        public SideMenuViewModel(
            IFileService fileService, 
            IModuleCatalog moduleCatalog, 
            IModuleManager moduleManager)
        {
            _fileService = fileService;
            _moduleCatalog = moduleCatalog;
            _moduleManager = moduleManager;

            GetDataCommand = new DelegateCommand(OnGetDataCommand);
        }

        #endregion

        #region Private Methods

        private void OnGetDataCommand()
        {
            Games = new ObservableCollection<InstalledGame>();

            var directories = _fileService.GetGameDirectories();

            foreach (var game in directories)
            {
                var moduleInfo = _moduleCatalog.Modules.SingleOrDefault(x => x.ModuleName == new DirectoryInfo(game).Name);
                var module = (InstalledGame)InstanceFactory.CreateInstance(Type.GetType(moduleInfo.ModuleType));
                Games.Add(module);
            }

            RaisePropertyChanged(nameof(Games));
        }

        private void ActivateModule(string name)
        {
            var module = _moduleCatalog.Modules.SingleOrDefault(x => x.ModuleName == name);
            _moduleManager.LoadModule(module.ModuleName);
        }

        #endregion
    }
}
