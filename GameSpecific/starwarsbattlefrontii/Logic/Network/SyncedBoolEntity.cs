using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SyncedBoolEntityData))]
	public class SyncedBoolEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SyncedBoolEntityData>
	{
		public new FrostySdk.Ebx.SyncedBoolEntityData Data => data as FrostySdk.Ebx.SyncedBoolEntityData;
		public override string DisplayName => "SyncedBool";

		public SyncedBoolEntity(FrostySdk.Ebx.SyncedBoolEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

