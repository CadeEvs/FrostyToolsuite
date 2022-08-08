using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterStateTriggerEntityData))]
	public class CharacterStateTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.CharacterStateTriggerEntityData>
	{
		public new FrostySdk.Ebx.CharacterStateTriggerEntityData Data => data as FrostySdk.Ebx.CharacterStateTriggerEntityData;

		public CharacterStateTriggerEntity(FrostySdk.Ebx.CharacterStateTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

