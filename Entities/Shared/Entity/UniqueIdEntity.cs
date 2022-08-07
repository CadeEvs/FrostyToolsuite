using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UniqueIdEntityData))]
	public class UniqueIdEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UniqueIdEntityData>
	{
		public new FrostySdk.Ebx.UniqueIdEntityData Data => data as FrostySdk.Ebx.UniqueIdEntityData;
		public override string DisplayName => "UniqueId";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UniqueIdEntity(FrostySdk.Ebx.UniqueIdEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

