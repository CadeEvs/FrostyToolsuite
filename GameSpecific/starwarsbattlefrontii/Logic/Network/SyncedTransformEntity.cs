using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SyncedTransformEntityData))]
	public class SyncedTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SyncedTransformEntityData>
	{
		public new FrostySdk.Ebx.SyncedTransformEntityData Data => data as FrostySdk.Ebx.SyncedTransformEntityData;
		public override string DisplayName => "SyncedTransform";

		public SyncedTransformEntity(FrostySdk.Ebx.SyncedTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

