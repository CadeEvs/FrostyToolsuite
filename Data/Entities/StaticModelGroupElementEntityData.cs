using FrostySdk.Attributes;
using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Data
{
    public class StaticModelGroupElementEntityData : FrostySdk.Ebx.EntityData
    {
        [EbxFieldMeta(EbxFieldType.Pointer, "ObjectBlueprint")]
        public FrostySdk.Ebx.PointerRef Blueprint { get; set; }
        [IsHidden]
        public FrostySdk.Ebx.PointerRef InternalBlueprint { get; set; }
        [EbxFieldMeta(EbxFieldType.Struct)]
        public FrostySdk.Ebx.LinearTransform Transform { get; set; } = new FrostySdk.Ebx.LinearTransform();
        [EbxFieldMeta(EbxFieldType.Pointer, "Asset")]
        public FrostySdk.Ebx.PointerRef ObjectVariation { get; set; }
        public uint ObjectVariationHash { get; set; }
        [EbxFieldMeta(EbxFieldType.Struct)]
        public FrostySdk.Ebx.RenderingOverrides RenderingOverrides { get; set; } = new FrostySdk.Ebx.RenderingOverrides();
        public FrostySdk.Ebx.RadiosityTypeOverride RadiosityTypeOverride { get; set; }
        public bool TerrainShaderNodesEnable { get; set; }
        public FrostySdk.Ebx.CString HavokShapeType { get; set; }

        [IsHidden]
        public int Index { get; set; }
        [IsHidden]
        public bool IsHavok { get; set; }
    }
}
