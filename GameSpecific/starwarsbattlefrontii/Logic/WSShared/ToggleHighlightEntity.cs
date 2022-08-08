using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ToggleHighlightEntityData))]
	public class ToggleHighlightEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ToggleHighlightEntityData>
	{
		public new FrostySdk.Ebx.ToggleHighlightEntityData Data => data as FrostySdk.Ebx.ToggleHighlightEntityData;
		public override string DisplayName => "ToggleHighlight";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ToggleHighlightEntity(FrostySdk.Ebx.ToggleHighlightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

