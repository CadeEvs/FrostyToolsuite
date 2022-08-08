using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StorytellerEntityData))]
	public class StorytellerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StorytellerEntityData>
	{
		public new FrostySdk.Ebx.StorytellerEntityData Data => data as FrostySdk.Ebx.StorytellerEntityData;
		public override string DisplayName => "Storyteller";

		public StorytellerEntity(FrostySdk.Ebx.StorytellerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

