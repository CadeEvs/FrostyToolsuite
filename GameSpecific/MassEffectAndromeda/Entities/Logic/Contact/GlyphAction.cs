using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GlyphActionData))]
	public class GlyphAction : LogicEntity, IEntityData<FrostySdk.Ebx.GlyphActionData>
	{
		public new FrostySdk.Ebx.GlyphActionData Data => data as FrostySdk.Ebx.GlyphActionData;
		public override string DisplayName => "GlyphAction";

		public GlyphAction(FrostySdk.Ebx.GlyphActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

