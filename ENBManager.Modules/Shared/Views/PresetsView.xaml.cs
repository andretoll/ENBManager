using ENBManager.Infrastructure.BusinessEntities;
using ENBManager.Modules.Shared.ViewModels;
using System.Windows.Controls;

namespace ENBManager.Modules.Shared.Views
{
    /// <summary>
    /// Interaction logic for PresetsView.xaml
    /// </summary>
    public partial class PresetsView : UserControl
    {
        public PresetsView(GameModule game)
        {
            InitializeComponent();

            DataContext = new PresetsViewModel(game);
        }
    }
}
