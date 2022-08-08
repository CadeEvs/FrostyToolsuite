using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SharedLockBaseEntityData))]
	public class SharedLockBaseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SharedLockBaseEntityData>
	{
		public new FrostySdk.Ebx.SharedLockBaseEntityData Data => data as FrostySdk.Ebx.SharedLockBaseEntityData;
		public override string DisplayName => "SharedLockBase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SharedLockBaseEntity(FrostySdk.Ebx.SharedLockBaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

