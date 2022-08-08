using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SubtitleEventCreatorEntityData))]
	public class SubtitleEventCreatorEntity : GameEventCreatorEntity, IEntityData<FrostySdk.Ebx.SubtitleEventCreatorEntityData>
	{
		public new FrostySdk.Ebx.SubtitleEventCreatorEntityData Data => data as FrostySdk.Ebx.SubtitleEventCreatorEntityData;
		public override string DisplayName => "SubtitleEventCreator";

		public SubtitleEventCreatorEntity(FrostySdk.Ebx.SubtitleEventCreatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

