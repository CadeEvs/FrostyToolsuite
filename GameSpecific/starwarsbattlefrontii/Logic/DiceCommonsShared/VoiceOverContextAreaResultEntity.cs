using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VoiceOverContextAreaResultEntityData))]
	public class VoiceOverContextAreaResultEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VoiceOverContextAreaResultEntityData>
	{
		public new FrostySdk.Ebx.VoiceOverContextAreaResultEntityData Data => data as FrostySdk.Ebx.VoiceOverContextAreaResultEntityData;
		public override string DisplayName => "VoiceOverContextAreaResult";

		public VoiceOverContextAreaResultEntity(FrostySdk.Ebx.VoiceOverContextAreaResultEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

