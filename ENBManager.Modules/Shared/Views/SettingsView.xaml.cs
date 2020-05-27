using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.ViewModels;
using System.Windows.Controls;

namespace ENBManager.Modules.Shared.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView(GameModule game)
        {
            InitializeComponent();

            DataContext = new SettingsViewModel(game);
        }
    }
}
