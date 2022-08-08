using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverLabelFilterEntityData))]
	public class VoiceOverLabelFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverLabelFilterEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverLabelFilterEntityData Data => data as FrostySdk.Ebx.VoiceOverLabelFilterEntityData;
		public override string DisplayName => "VoiceOverLabelFilter";

		public VoiceOverLabelFilterEntity(FrostySdk.Ebx.VoiceOverLabelFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

