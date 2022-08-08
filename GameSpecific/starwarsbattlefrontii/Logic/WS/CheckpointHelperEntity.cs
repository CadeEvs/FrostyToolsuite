using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CheckpointHelperEntityData))]
	public class CheckpointHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CheckpointHelperEntityData>
	{
		public new FrostySdk.Ebx.CheckpointHelperEntityData Data => data as FrostySdk.Ebx.CheckpointHelperEntityData;
		public override string DisplayName => "CheckpointHelper";

		public CheckpointHelperEntity(FrostySdk.Ebx.CheckpointHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

