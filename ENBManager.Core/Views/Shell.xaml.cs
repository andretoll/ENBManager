using ENBManager.Core.ViewModels;
using System;
using System.Windows;

namespace ENBManager.Core.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        #region Private Members

        private ShellViewModel _viewModel;

        #endregion

        public Shell()
        {
            InitializeComponent();

            StateChanged += Shell_StateChanged;

            _viewModel = DataContext as ShellViewModel;
            _viewModel.ExitApplicationEventHandler += ViewModel_ExitApplicationEventHandler;
            _viewModel.RestoreApplicationEventHandler += _viewModel_RestoreApplicationEventHandler;
        }

        #region Events

        private void Shell_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Minimized:
                    Minimize();
                    break;
                case WindowState.Normal:
                    RestoreWindow();
                    break;
            }
        }

        private void ViewModel_ExitApplicationEventHandler(object sender, EventArgs e)
        {
            ExitApplication();
        }

        private void _viewModel_RestoreApplicationEventHandler(object sender, EventArgs e)
        {
            RestoreWindow();
        }

        #endregion

        #region Private Methods

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        private void Minimize()
        {
            _viewModel.ShowNotifyIcon = true;

            if (_viewModel.MinimizeToTray)
                Hide();
        }

        private void RestoreWindow()
        {
            _viewModel.ShowNotifyIcon = false;

            if (Visibility != Visibility.Visible)
                Show();

            WindowState = WindowState.Normal;
        }

        #endregion
    }
}
