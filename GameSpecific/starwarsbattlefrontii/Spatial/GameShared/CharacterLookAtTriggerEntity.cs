using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterLookAtTriggerEntityData))]
	public class CharacterLookAtTriggerEntity : TriggerEventEntity, IEntityData<FrostySdk.Ebx.CharacterLookAtTriggerEntityData>
	{
		public new FrostySdk.Ebx.CharacterLookAtTriggerEntityData Data => data as FrostySdk.Ebx.CharacterLookAtTriggerEntityData;

		public CharacterLookAtTriggerEntity(FrostySdk.Ebx.CharacterLookAtTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

