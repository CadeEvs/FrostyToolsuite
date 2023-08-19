using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using FrostyEditor.Models;

namespace FrostyEditor.ViewModels.Tools;

public partial class DataExplorerViewModel : Tool
{
    [ObservableProperty]
    private string m_test = "Explorer";

    [ObservableProperty]
    private FlatTreeDataGridSource<AssetModel> m_assetsSource;
    
    public HierarchicalTreeDataGridSource<FolderTreeNodeModel> FolderSource { get; }

    public DataExplorerViewModel()
    {
        FolderSource = new HierarchicalTreeDataGridSource<FolderTreeNodeModel>(FolderTreeNodeModel.Create())
        {
            Columns =
            {
                new HierarchicalExpanderColumn<FolderTreeNodeModel>(
                    new TextColumn<FolderTreeNodeModel,string>(
                        "Name",
                        x => x.Name,
                        new GridLength(1, GridUnitType.Star),
                        options: new TextColumnOptions<FolderTreeNodeModel>
                        {
                            CanUserResizeColumn = false,
                            CanUserSortColumn = false,
                            CompareAscending = FolderTreeNodeModel.SortAscending(x => x.Name),
                            CompareDescending = FolderTreeNodeModel.SortDescending(x => x.Name),
                        }),
                    x => x.Children,
                    x => x.HasChildren,
                    x => x.IsExpanded),
            }
        };

        FolderSource.RowSelection!.SelectionChanged += OnSelectionChanged;
        FolderSource.Sort(FolderTreeNodeModel.SortAscending(x => x.Name));
        
        AssetsSource = new FlatTreeDataGridSource<AssetModel>(Array.Empty<AssetModel>())
        {
            Columns =
            {
                new TextColumn<AssetModel, string>(
                    "Name",
                    x => x.Name,
                    new GridLength(2, GridUnitType.Star),
                    new TextColumnOptions<AssetModel>()
                    {
                        CompareAscending = AssetModel.SortAscending(x => x.Name),
                        CompareDescending = AssetModel.SortDescending(x => x.Name),
                    }),
                new TextColumn<AssetModel, string>(
                    "Type",
                    x => x.Type,
                    new GridLength(1, GridUnitType.Star),
                    new TextColumnOptions<AssetModel>()
                    {
                        CompareAscending = AssetModel.SortAscending(x => x.Type),
                        CompareDescending = AssetModel.SortDescending(x => x.Type),
                    })
            }
        };
        
    }

    private void OnSelectionChanged(object? sender, TreeSelectionModelSelectionChangedEventArgs<FolderTreeNodeModel> e)
    {
        FolderTreeNodeModel? b = e.SelectedItems[0];
        if (b is null)
        {
            return;
        }

        AssetsSource.Items = b.Assets;
    }
}
