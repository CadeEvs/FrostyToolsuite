using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SharedLockControllerEntityData))]
	public class SharedLockControllerEntity : SharedLockBaseEntity, IEntityData<FrostySdk.Ebx.SharedLockControllerEntityData>
	{
		public new FrostySdk.Ebx.SharedLockControllerEntityData Data => data as FrostySdk.Ebx.SharedLockControllerEntityData;
		public override string DisplayName => "SharedLockController";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SharedLockControllerEntity(FrostySdk.Ebx.SharedLockControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

