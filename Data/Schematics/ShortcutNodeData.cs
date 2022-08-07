using FrostySdk.Attributes;
using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Data
{
    public interface IShortcutNodeData
    {
        string DisplayName { get; set; }
    }

    [DisplayName("ShortcutData")]
    public class ShortcutParentNodeData : IShortcutNodeData
    {
        [Category("Shortcut")]
        [EbxFieldMeta(EbxFieldType.String)]
        public string DisplayName { get; set; }
    }

    [DisplayName("ShortcutData")]
    public class ShortcutChildNodeData : IShortcutNodeData
    {
        [IsReadOnly]
        [Category("Shortcut")]
        [EbxFieldMeta(EbxFieldType.String)]
        public string DisplayName { get; set; }
    }
}
