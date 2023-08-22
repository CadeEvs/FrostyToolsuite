using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using FrostyEditor.Models;

namespace FrostyEditor.ViewModels.Tools;

public partial class DataExplorerViewModel : Tool
{
    public static IMultiValueConverter FolderIconConverter
    {
        get
        {
            if (s_folderIconConverter is null)
            {
                using (var folderCollapsedStream = AssetLoader.Open(new Uri("avares://FrostyEditor/Assets/FolderCollapsed.png")))
                using (var folderExpandStream = AssetLoader.Open(new Uri("avares://FrostyEditor/Assets/FolderExpanded.png")))
                {
                    var folderCollapsedIcon = new Bitmap(folderCollapsedStream);
                    var folderExpandIcon = new Bitmap(folderExpandStream);

                    s_folderIconConverter = new FolderIconConvert(folderExpandIcon, folderCollapsedIcon);
                }
            }

            return s_folderIconConverter;
        }
    }

    public HierarchicalTreeDataGridSource<FolderTreeNodeModel> FolderSource { get; }

    private static FolderIconConvert? s_folderIconConverter;

    [ObservableProperty]
    private string m_test = "Explorer";

    [ObservableProperty]
    private FlatTreeDataGridSource<AssetModel> m_assetsSource;

    public DataExplorerViewModel()
    {
        FolderSource = new HierarchicalTreeDataGridSource<FolderTreeNodeModel>(FolderTreeNodeModel.Create())
        {
            Columns =
            {
                new HierarchicalExpanderColumn<FolderTreeNodeModel>(
                    new TemplateColumn<FolderTreeNodeModel>(
                        "Name",
                        "FolderNameCell",
                        null,
                        new GridLength(1, GridUnitType.Star),
                        options: new()
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

    private class FolderIconConvert : IMultiValueConverter
    {
        private readonly Bitmap m_folderExpanded;
        private readonly Bitmap m_folderCollapsed;
        public FolderIconConvert(Bitmap folderExpanded, Bitmap folderCollapsed)
        {
            m_folderExpanded = folderExpanded;
            m_folderCollapsed = folderCollapsed;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count == 1 &&
                values[0] is bool isExpanded)
            {
                return isExpanded ? m_folderExpanded : m_folderCollapsed;
            }

            return null;
        }
    }
}
