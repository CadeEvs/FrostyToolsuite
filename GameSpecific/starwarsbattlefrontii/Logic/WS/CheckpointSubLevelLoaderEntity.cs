using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CheckpointSubLevelLoaderEntityData))]
	public class CheckpointSubLevelLoaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CheckpointSubLevelLoaderEntityData>
	{
		public new FrostySdk.Ebx.CheckpointSubLevelLoaderEntityData Data => data as FrostySdk.Ebx.CheckpointSubLevelLoaderEntityData;
		public override string DisplayName => "CheckpointSubLevelLoader";

		public CheckpointSubLevelLoaderEntity(FrostySdk.Ebx.CheckpointSubLevelLoaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

