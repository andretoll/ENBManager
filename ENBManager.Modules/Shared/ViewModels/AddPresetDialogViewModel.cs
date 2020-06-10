using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Infrastructure.BusinessEntities.Base;
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
        private DirectoryNode _selectedDirectory;

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

        public ObservableCollection<Node> Items { get; set; }

        public DirectoryNode SelectedDirectory
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

        public bool IsFormValid => !string.IsNullOrWhiteSpace(Name) && Items?.Count > 0;

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
                _logger.Debug("Node selected/deselected");

                if (sender.GetType() == typeof(FileNode))
                    SelectedDirectory = null;
                else if (sender.GetType() == typeof(DirectoryNode))
                {
                    if ((sender as DirectoryNode).Path == Items.Single().Path)
                        SelectedDirectory = null;
                    else if ((sender as DirectoryNode).IsSelected)
                        SelectedDirectory = sender as DirectoryNode;
                }
            }
        }

        #endregion

        #region Private Methods

        private async Task OnSaveCommand()
        {
            _logger.Debug(nameof(OnSaveCommand));

            if (string.IsNullOrEmpty(_name) || Items == null || Items.Count == 0)
                return;

            // Create preset
            var preset = new Preset
            {
                Name = Name,
                FullPath = Items.Single().Path,

                // Map paths
                Files = TreeViewHelper.GetPaths(Items.Single() as DirectoryNode)
            };

            // Save preset
            using (var dialog = new ProgressDialog(true))
            {
                try
                {
                    dialog.SetHost(RegionNames.AddPresetDialogHost);
                    _ = dialog.OpenAsync();
                    await _presetManager.SaveNewPresetAsync(Paths.GetPresetsDirectory(_game.Module), preset);

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
                catch (IOException ex)
                {
                    var messageDialog = new MessageDialog(Strings.INVALID_NAME);
                    messageDialog.SetHost(RegionNames.AddPresetDialogHost);
                    await messageDialog.OpenAsync();
                    _logger.Warn(ex);
                    return;
                }
            }

            var dp = new DialogParameters
            {
                { "PresetName", preset.Name }
            };
            RequestClose.Invoke(new DialogResult(ButtonResult.OK, dp));
        }

        private void OnBrowseFolderCommand()
        {
            _logger.Debug(nameof(OnBrowseFolderCommand));

            bool accepted = DialogHelper.OpenFolderDialog(out string path);
            if (!accepted)
                return;

            // Create root node
            Items = new ObservableCollection<Node>();
            var root = new DirectoryNode
            {
                Path = path,
                Name = Path.GetFileName(path)
            };
            root.PropertyChanged += Item_PropertyChanged;

            // Get nodes
            root.Items = new ObservableCollection<Node>(TreeViewHelper.GetItems(root.Path, Item_PropertyChanged));
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
            _logger.Debug(nameof(OnSetRootDirectoryCommand));

            if (!IsDirectorySelected)
                return;

            // Create root node
            Items = new ObservableCollection<Node>();
            var root = new DirectoryNode
            {
                Path = SelectedDirectory.Path,
                Name = Path.GetFileName(SelectedDirectory.Path)
            };
            root.PropertyChanged += Item_PropertyChanged;

            // Get nodes
            root.Items = new ObservableCollection<Node>(TreeViewHelper.GetItems(root.Path, Item_PropertyChanged));
            Items.Add(root);

            RaisePropertyChanged(nameof(Items));
            RaisePropertyChanged(nameof(IsFormValid));
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
