using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SyncedIntEntityData))]
	public class SyncedIntEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SyncedIntEntityData>
	{
		public new FrostySdk.Ebx.SyncedIntEntityData Data => data as FrostySdk.Ebx.SyncedIntEntityData;
		public override string DisplayName => "SyncedInt";

		public SyncedIntEntity(FrostySdk.Ebx.SyncedIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

