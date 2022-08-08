using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverSubstituteSourceListEntityData))]
	public class VoiceOverSubstituteSourceListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverSubstituteSourceListEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverSubstituteSourceListEntityData Data => data as FrostySdk.Ebx.VoiceOverSubstituteSourceListEntityData;
		public override string DisplayName => "VoiceOverSubstituteSourceList";

		public VoiceOverSubstituteSourceListEntity(FrostySdk.Ebx.VoiceOverSubstituteSourceListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

