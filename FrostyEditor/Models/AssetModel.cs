using System;
using System.Collections.Generic;
using Frosty.Sdk.Managers.Entries;

namespace FrostyEditor.Models;

public class AssetModel
{
    public string? Name => m_entry?.Filename;
    public string? Type => m_entry?.Type;
    
    private readonly AssetEntry? m_entry;

    public AssetModel(AssetEntry inEntry)
    {
        m_entry = inEntry;
    }
    
    public static Comparison<AssetModel?> SortAscending<T>(Func<AssetModel, T> selector)
    {
        return (x, y) =>
        {
            if (x is null && y is null)
                return 0;
            if (x is null)
                return -1;
            if (y is null)
                return 1;

            return Comparer<T>.Default.Compare(selector(x), selector(y));
        };
    }

    public static Comparison<AssetModel?> SortDescending<T>(Func<AssetModel, T> selector)
    {
        return (x, y) =>
        {
            if (x is null && y is null)
                return 0;
            if (x is null)
                return 1;
            if (y is null)
                return -1;
            return Comparer<T>.Default.Compare(selector(y), selector(x));
        };
    }
}