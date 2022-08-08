using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectVariationToIntEntityData))]
	public class ObjectVariationToIntEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectVariationToIntEntityData>
	{
		public new FrostySdk.Ebx.ObjectVariationToIntEntityData Data => data as FrostySdk.Ebx.ObjectVariationToIntEntityData;
		public override string DisplayName => "ObjectVariationToInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ObjectVariationToIntEntity(FrostySdk.Ebx.ObjectVariationToIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

