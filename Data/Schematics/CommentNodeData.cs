using FrostySdk.Attributes;
using FrostySdk.IO;
using LevelEditorPlugin.Controls.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Data
{
    public class CommentNodeData
    {
        [Category("Comment")]
        [EbxFieldMeta(EbxFieldType.Struct)]
        [Editor(typeof(ColorPickerEditor))]
        public FrostySdk.Ebx.Vec4 Color { get; set; } = new FrostySdk.Ebx.Vec4();

        [Category("Comment")]
        [EbxFieldMeta(EbxFieldType.String)]
        public string CommentText { get; set; }
    }
}
