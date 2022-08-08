using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KitLimitationLockEntityData))]
	public class KitLimitationLockEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KitLimitationLockEntityData>
	{
		public new FrostySdk.Ebx.KitLimitationLockEntityData Data => data as FrostySdk.Ebx.KitLimitationLockEntityData;
		public override string DisplayName => "KitLimitationLock";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public KitLimitationLockEntity(FrostySdk.Ebx.KitLimitationLockEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

