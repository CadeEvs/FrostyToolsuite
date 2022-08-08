using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LookAtTriggerEntityData))]
	public class LookAtTriggerEntity : CharacterLookAtTriggerEntity, IEntityData<FrostySdk.Ebx.LookAtTriggerEntityData>
	{
		public new FrostySdk.Ebx.LookAtTriggerEntityData Data => data as FrostySdk.Ebx.LookAtTriggerEntityData;

		public LookAtTriggerEntity(FrostySdk.Ebx.LookAtTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

