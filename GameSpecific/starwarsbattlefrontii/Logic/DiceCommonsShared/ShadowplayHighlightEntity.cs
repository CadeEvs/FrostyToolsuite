using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShadowplayHighlightEntityData))]
	public class ShadowplayHighlightEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ShadowplayHighlightEntityData>
	{
		public new FrostySdk.Ebx.ShadowplayHighlightEntityData Data => data as FrostySdk.Ebx.ShadowplayHighlightEntityData;
		public override string DisplayName => "ShadowplayHighlight";

		public ShadowplayHighlightEntity(FrostySdk.Ebx.ShadowplayHighlightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

