using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SyncedVec3EntityData))]
	public class SyncedVec3Entity : LogicEntity, IEntityData<FrostySdk.Ebx.SyncedVec3EntityData>
	{
		public new FrostySdk.Ebx.SyncedVec3EntityData Data => data as FrostySdk.Ebx.SyncedVec3EntityData;
		public override string DisplayName => "SyncedVec3";

		public SyncedVec3Entity(FrostySdk.Ebx.SyncedVec3EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

