using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClearAreaTriggerEntityData))]
	public class ClearAreaTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.ClearAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.ClearAreaTriggerEntityData Data => data as FrostySdk.Ebx.ClearAreaTriggerEntityData;

		public ClearAreaTriggerEntity(FrostySdk.Ebx.ClearAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

