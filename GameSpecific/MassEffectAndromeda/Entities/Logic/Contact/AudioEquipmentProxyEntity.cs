using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioEquipmentProxyEntityData))]
	public class AudioEquipmentProxyEntity : AudioAssetEntity, IEntityData<FrostySdk.Ebx.AudioEquipmentProxyEntityData>
	{
		public new FrostySdk.Ebx.AudioEquipmentProxyEntityData Data => data as FrostySdk.Ebx.AudioEquipmentProxyEntityData;
		public override string DisplayName => "AudioEquipmentProxy";

		public AudioEquipmentProxyEntity(FrostySdk.Ebx.AudioEquipmentProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

