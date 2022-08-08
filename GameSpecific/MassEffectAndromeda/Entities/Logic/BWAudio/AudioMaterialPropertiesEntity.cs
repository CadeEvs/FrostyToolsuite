using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioMaterialPropertiesEntityData))]
	public class AudioMaterialPropertiesEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioMaterialPropertiesEntityData>
	{
		public new FrostySdk.Ebx.AudioMaterialPropertiesEntityData Data => data as FrostySdk.Ebx.AudioMaterialPropertiesEntityData;
		public override string DisplayName => "AudioMaterialProperties";

		public AudioMaterialPropertiesEntity(FrostySdk.Ebx.AudioMaterialPropertiesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

