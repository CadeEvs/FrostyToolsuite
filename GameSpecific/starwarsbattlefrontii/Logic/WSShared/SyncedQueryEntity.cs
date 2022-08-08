using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SyncedQueryEntityData))]
	public class SyncedQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SyncedQueryEntityData>
	{
		public new FrostySdk.Ebx.SyncedQueryEntityData Data => data as FrostySdk.Ebx.SyncedQueryEntityData;
		public override string DisplayName => "SyncedQuery";

		public SyncedQueryEntity(FrostySdk.Ebx.SyncedQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

