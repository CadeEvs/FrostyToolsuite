using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StorytellerMockEntityData))]
	public class StorytellerMockEntity : StorytellerEntity, IEntityData<FrostySdk.Ebx.StorytellerMockEntityData>
	{
		public new FrostySdk.Ebx.StorytellerMockEntityData Data => data as FrostySdk.Ebx.StorytellerMockEntityData;
		public override string DisplayName => "StorytellerMock";

		public StorytellerMockEntity(FrostySdk.Ebx.StorytellerMockEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

