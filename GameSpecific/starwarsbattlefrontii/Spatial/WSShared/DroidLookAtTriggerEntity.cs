using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidLookAtTriggerEntityData))]
	public class DroidLookAtTriggerEntity : CharacterLookAtTriggerEntity, IEntityData<FrostySdk.Ebx.DroidLookAtTriggerEntityData>
	{
		public new FrostySdk.Ebx.DroidLookAtTriggerEntityData Data => data as FrostySdk.Ebx.DroidLookAtTriggerEntityData;

		public DroidLookAtTriggerEntity(FrostySdk.Ebx.DroidLookAtTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

