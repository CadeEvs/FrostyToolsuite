using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientVoiceOverQueryEventEntityData))]
	public class ClientVoiceOverQueryEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientVoiceOverQueryEventEntityData>
	{
		public new FrostySdk.Ebx.ClientVoiceOverQueryEventEntityData Data => data as FrostySdk.Ebx.ClientVoiceOverQueryEventEntityData;
		public override string DisplayName => "ClientVoiceOverQueryEvent";

		public ClientVoiceOverQueryEventEntity(FrostySdk.Ebx.ClientVoiceOverQueryEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

