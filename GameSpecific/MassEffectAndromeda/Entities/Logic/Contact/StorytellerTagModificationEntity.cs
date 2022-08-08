using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StorytellerTagModificationEntityData))]
	public class StorytellerTagModificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StorytellerTagModificationEntityData>
	{
		public new FrostySdk.Ebx.StorytellerTagModificationEntityData Data => data as FrostySdk.Ebx.StorytellerTagModificationEntityData;
		public override string DisplayName => "StorytellerTagModification";

		public StorytellerTagModificationEntity(FrostySdk.Ebx.StorytellerTagModificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

