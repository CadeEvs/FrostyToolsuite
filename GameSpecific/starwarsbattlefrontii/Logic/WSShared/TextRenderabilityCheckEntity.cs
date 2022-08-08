using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextRenderabilityCheckEntityData))]
	public class TextRenderabilityCheckEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TextRenderabilityCheckEntityData>
	{
		public new FrostySdk.Ebx.TextRenderabilityCheckEntityData Data => data as FrostySdk.Ebx.TextRenderabilityCheckEntityData;
		public override string DisplayName => "TextRenderabilityCheck";

		public TextRenderabilityCheckEntity(FrostySdk.Ebx.TextRenderabilityCheckEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

