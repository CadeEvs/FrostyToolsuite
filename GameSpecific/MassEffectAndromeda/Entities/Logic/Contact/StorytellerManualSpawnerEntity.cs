using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StorytellerManualSpawnerEntityData))]
	public class StorytellerManualSpawnerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StorytellerManualSpawnerEntityData>
	{
		public new FrostySdk.Ebx.StorytellerManualSpawnerEntityData Data => data as FrostySdk.Ebx.StorytellerManualSpawnerEntityData;
		public override string DisplayName => "StorytellerManualSpawner";

		public StorytellerManualSpawnerEntity(FrostySdk.Ebx.StorytellerManualSpawnerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

