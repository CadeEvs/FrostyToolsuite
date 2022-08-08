using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SharedLockGateEntityData))]
	public class SharedLockGateEntity : SharedLockBaseEntity, IEntityData<FrostySdk.Ebx.SharedLockGateEntityData>
	{
		public new FrostySdk.Ebx.SharedLockGateEntityData Data => data as FrostySdk.Ebx.SharedLockGateEntityData;
		public override string DisplayName => "SharedLockGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SharedLockGateEntity(FrostySdk.Ebx.SharedLockGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

