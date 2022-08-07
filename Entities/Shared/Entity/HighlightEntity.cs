using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HighlightEntityData))]
	public class HighlightEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HighlightEntityData>
	{
		public new FrostySdk.Ebx.HighlightEntityData Data => data as FrostySdk.Ebx.HighlightEntityData;
		public override string DisplayName => "Highlight";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public HighlightEntity(FrostySdk.Ebx.HighlightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

