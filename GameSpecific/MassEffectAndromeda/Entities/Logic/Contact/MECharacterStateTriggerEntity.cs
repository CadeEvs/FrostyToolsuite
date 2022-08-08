using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECharacterStateTriggerEntityData))]
	public class MECharacterStateTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MECharacterStateTriggerEntityData>
	{
		public new FrostySdk.Ebx.MECharacterStateTriggerEntityData Data => data as FrostySdk.Ebx.MECharacterStateTriggerEntityData;
		public override string DisplayName => "MECharacterStateTrigger";

		public MECharacterStateTriggerEntity(FrostySdk.Ebx.MECharacterStateTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

