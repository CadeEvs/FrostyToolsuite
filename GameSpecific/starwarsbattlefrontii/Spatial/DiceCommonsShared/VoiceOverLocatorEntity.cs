using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverLocatorEntityData))]
	public class VoiceOverLocatorEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.VoiceOverLocatorEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverLocatorEntityData Data => data as FrostySdk.Ebx.VoiceOverLocatorEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public VoiceOverLocatorEntity(FrostySdk.Ebx.VoiceOverLocatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

