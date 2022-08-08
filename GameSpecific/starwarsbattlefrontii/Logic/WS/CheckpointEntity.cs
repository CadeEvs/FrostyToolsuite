using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CheckpointEntityData))]
	public class CheckpointEntity : StartPointEntity, IEntityData<FrostySdk.Ebx.CheckpointEntityData>
	{
		public new FrostySdk.Ebx.CheckpointEntityData Data => data as FrostySdk.Ebx.CheckpointEntityData;
		public override string DisplayName => "Checkpoint";

		public CheckpointEntity(FrostySdk.Ebx.CheckpointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

