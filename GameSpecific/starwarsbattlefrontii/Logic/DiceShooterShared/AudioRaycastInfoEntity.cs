using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioRaycastInfoEntityData))]
	public class AudioRaycastInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioRaycastInfoEntityData>
	{
		public new FrostySdk.Ebx.AudioRaycastInfoEntityData Data => data as FrostySdk.Ebx.AudioRaycastInfoEntityData;
		public override string DisplayName => "AudioRaycastInfo";

		public AudioRaycastInfoEntity(FrostySdk.Ebx.AudioRaycastInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

