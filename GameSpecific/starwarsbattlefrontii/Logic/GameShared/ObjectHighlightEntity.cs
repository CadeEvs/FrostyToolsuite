using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectHighlightEntityData))]
	public class ObjectHighlightEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectHighlightEntityData>
	{
		public new FrostySdk.Ebx.ObjectHighlightEntityData Data => data as FrostySdk.Ebx.ObjectHighlightEntityData;
		public override string DisplayName => "ObjectHighlight";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ObjectHighlightEntity(FrostySdk.Ebx.ObjectHighlightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

