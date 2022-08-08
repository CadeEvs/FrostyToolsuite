using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioWheelLogicEntityData))]
	public class AudioWheelLogicEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioWheelLogicEntityData>
	{
		public new FrostySdk.Ebx.AudioWheelLogicEntityData Data => data as FrostySdk.Ebx.AudioWheelLogicEntityData;
		public override string DisplayName => "AudioWheelLogic";

		public AudioWheelLogicEntity(FrostySdk.Ebx.AudioWheelLogicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

