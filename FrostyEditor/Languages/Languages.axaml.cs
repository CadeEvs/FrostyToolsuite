using System;
using System.Collections.Generic;

namespace FrostyEditor.Languages;

public class LanguageList : List<LangEntry>
{
    public static int TotalLines;
}

public class LangEntry
{
    public string FileName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Lines { get; set; }
    public bool Local { get; set; } = false;

    public override string ToString()
    {
        return Lines != LanguageList.TotalLines ? $"{Name} ({Math.Floor((float)Lines / LanguageList.TotalLines * 100)}%)" : Name;
    }
}