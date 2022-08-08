using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoolChangedEntityData))]
	public class BoolChangedEntity : PropertyChangedEntity, IEntityData<FrostySdk.Ebx.BoolChangedEntityData>
	{
		public new FrostySdk.Ebx.BoolChangedEntityData Data => data as FrostySdk.Ebx.BoolChangedEntityData;
		public override string DisplayName => "BoolChanged";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BoolChangedEntity(FrostySdk.Ebx.BoolChangedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

