using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.BusinessEntities.Dialogs;
using ENBManager.Infrastructure.Constants;
using ENBManager.Infrastructure.Exceptions;
using ENBManager.Infrastructure.Helpers;
using ENBManager.Localization.Strings;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Models;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ENBManager.Modules.Shared.ViewModels
{
    public class AddPresetDialogViewModel : BindableBase, IDialogAware
    {
        #region Private Members

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IPresetManager _presetManager;

        private GameModule _game;
        private string _name;
        private DirectoryItem _selectedDirectory;

        #endregion

        #region Public Properties

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();

                if (Items == null)
                    return;

                Items.Single().Name = value;
                RaisePropertyChanged(nameof(Items));
                RaisePropertyChanged(nameof(IsFormValid));
            }
        }

        public ObservableCollection<Item> Items { get; set; }

        public DirectoryItem SelectedDirectory
        {
            get { return _selectedDirectory; }
            set
            {
                _selectedDirectory = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDirectorySelected));
            }
        }
        public bool IsDirectorySelected => SelectedDirectory != null;
        public bool IsFormValid => Name?.Length > 0 && Items?.Count > 0;

        #endregion

        #region Commands

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand BrowseFolderCommand { get; }
        public DelegateCommand SetRootDirectoryCommand { get; }

        #endregion

        #region Constructor

        public AddPresetDialogViewModel(IPresetManager presetManager)
        {
            _presetManager = presetManager;

            SaveCommand = new DelegateCommand(async () => await OnSaveCommand(), () => IsFormValid).ObservesCanExecute(() => IsFormValid);
            CancelCommand = new DelegateCommand(() => RequestClose.Invoke(new DialogResult(ButtonResult.Cancel)));
            BrowseFolderCommand = new DelegateCommand(OnBrowseFolderCommand);
            SetRootDirectoryCommand = new DelegateCommand(OnSetRootDirectoryCommand, () => SelectedDirectory != null).ObservesCanExecute(() => IsDirectorySelected);
        }

        #endregion

        #region Events

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                if (sender.GetType() == typeof(FileItem))
                    SelectedDirectory = null;
                else if (sender.GetType() == typeof(DirectoryItem))
                {
                    if ((sender as DirectoryItem).Path == Items.Single().Path)
                        SelectedDirectory = null;
                    else if ((sender as DirectoryItem).IsSelected)
                        SelectedDirectory = sender as DirectoryItem;
                }
            }
        }

        #endregion

        #region Private Methods

        private async Task OnSaveCommand()
        {
            if (string.IsNullOrEmpty(_name) || Items == null || Items.Count == 0)
                return;

            // Create preset
            var preset = new Preset();
            preset.Name = Name;
            preset.FullPath = Items.Single().Path;

            // Map paths
            preset.Files = GetPaths(Items.Single() as DirectoryItem);

            // Save preset
            using (var dialog = new ProgressDialog(true))
            {
                try
                {
                    dialog.SetHost(RegionNames.AddPresetDialogHost);
                    _ = dialog.OpenAsync();
                    await _presetManager.SaveNewPresetAsync(_game, preset);

                    _logger.Info($"Preset {preset.Name} added");
                }
                catch (IdenticalNameException ex)
                {
                    var messageDialog = new MessageDialog(Strings.AN_ITEM_WITH_THIS_NAME_ALREADY_EXISTS);
                    messageDialog.SetHost(RegionNames.AddPresetDialogHost);
                    await messageDialog.OpenAsync();
                    _logger.Debug(ex.Message);
                    return;
                } 
            }
            
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void OnBrowseFolderCommand()
        {
            bool accepted = DialogHelper.OpenFolderDialog(out string path);
            if (!accepted)
                return;

            Items = new ObservableCollection<Item>();
            var root = new DirectoryItem
            {
                Path = path,
                Name = Path.GetFileName(path)
            };
            root.PropertyChanged += Item_PropertyChanged;

            root.Items = new ObservableCollection<Item>(GetItems(root.Path));
            Items.Add(root);

            RaisePropertyChanged(nameof(Items));
            RaisePropertyChanged(nameof(IsFormValid));

            if (string.IsNullOrEmpty(Name))
                Name = root.Name;
            else
                root.Name = Name;
        }

        private void OnSetRootDirectoryCommand()
        {
            if (!IsDirectorySelected)
                return;

            Items = new ObservableCollection<Item>();
            var root = new DirectoryItem
            {
                Path = SelectedDirectory.Path,
                Name = Path.GetFileName(SelectedDirectory.Path)
            };

            root.Items = new ObservableCollection<Item>(GetItems(root.Path));
            Items.Add(root);

            RaisePropertyChanged(nameof(Items));
        }

        #endregion

        #region Helper Methods

        private List<Item> GetItems(string root)
        {
            var items = new List<Item>();

            var dirInfo = new DirectoryInfo(root);  

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryItem
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    Items = new ObservableCollection<Item>(GetItems(directory.FullName))
                };

                item.PropertyChanged += Item_PropertyChanged;

                items.Add(item);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var item = new FileItem
                {
                    Name = file.Name,
                    Path = file.FullName
                };

                item.PropertyChanged += Item_PropertyChanged;

                items.Add(item);
            }

            return items;
        }

        private IEnumerable<string> GetPaths(DirectoryItem directory)
        {
            List<string> paths = new List<string>();

            // Add empty directory
            if (directory.Items.Count == 0)
                paths.Add(directory.Path);

            foreach (var item in directory.Items.Where(x => x.GetType() == typeof(DirectoryItem)))
            {
                paths.AddRange(GetPaths(item as DirectoryItem));
            }

            foreach (var file in directory.Items.Where(x => x.GetType() == typeof(FileItem)))
            {
                paths.Add(file.Path);
            }

            return paths;
        }

        #endregion

        #region IDialogAware Implementation

        public string Title => Strings.ADD_NEW_PRESET;

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            _logger.Info(nameof(OnDialogClosed));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.GetValue<GameModule>("GameModule") != null)
            {
                _game = parameters.GetValue<GameModule>("GameModule");
            }

            _logger.Info(nameof(OnDialogOpened));
        } 

        #endregion
    }
}
