using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;

namespace FrostyEditor.ViewModels.Tools;

public partial class DataExplorerViewModel : Tool
{
    [ObservableProperty]
    private string m_test = "Explorer";
}
