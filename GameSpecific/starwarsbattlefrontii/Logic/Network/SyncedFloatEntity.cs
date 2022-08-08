using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SyncedFloatEntityData))]
	public class SyncedFloatEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SyncedFloatEntityData>
	{
		public new FrostySdk.Ebx.SyncedFloatEntityData Data => data as FrostySdk.Ebx.SyncedFloatEntityData;
		public override string DisplayName => "SyncedFloat";

		public SyncedFloatEntity(FrostySdk.Ebx.SyncedFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

