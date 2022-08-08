using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsLockHeightVolumeEntityData))]
	public class PhysicsLockHeightVolumeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsLockHeightVolumeEntityData>
	{
		public new FrostySdk.Ebx.PhysicsLockHeightVolumeEntityData Data => data as FrostySdk.Ebx.PhysicsLockHeightVolumeEntityData;
		public override string DisplayName => "PhysicsLockHeightVolume";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsLockHeightVolumeEntity(FrostySdk.Ebx.PhysicsLockHeightVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

