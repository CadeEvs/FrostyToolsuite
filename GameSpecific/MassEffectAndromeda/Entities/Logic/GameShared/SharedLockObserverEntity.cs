using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SharedLockObserverEntityData))]
	public class SharedLockObserverEntity : SharedLockBaseEntity, IEntityData<FrostySdk.Ebx.SharedLockObserverEntityData>
	{
		public new FrostySdk.Ebx.SharedLockObserverEntityData Data => data as FrostySdk.Ebx.SharedLockObserverEntityData;
		public override string DisplayName => "SharedLockObserver";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SharedLockObserverEntity(FrostySdk.Ebx.SharedLockObserverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

