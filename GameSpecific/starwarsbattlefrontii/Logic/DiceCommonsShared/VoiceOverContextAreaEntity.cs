using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverContextAreaEntityData))]
	public class VoiceOverContextAreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverContextAreaEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverContextAreaEntityData Data => data as FrostySdk.Ebx.VoiceOverContextAreaEntityData;
		public override string DisplayName => "VoiceOverContextArea";

		public VoiceOverContextAreaEntity(FrostySdk.Ebx.VoiceOverContextAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

