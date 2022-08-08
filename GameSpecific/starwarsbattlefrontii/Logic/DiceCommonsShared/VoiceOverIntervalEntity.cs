using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverIntervalEntityData))]
	public class VoiceOverIntervalEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverIntervalEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverIntervalEntityData Data => data as FrostySdk.Ebx.VoiceOverIntervalEntityData;
		public override string DisplayName => "VoiceOverInterval";

		public VoiceOverIntervalEntity(FrostySdk.Ebx.VoiceOverIntervalEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

