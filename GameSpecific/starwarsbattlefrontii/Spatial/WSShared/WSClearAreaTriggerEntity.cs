using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSClearAreaTriggerEntityData))]
	public class WSClearAreaTriggerEntity : ClearAreaTriggerEntity, IEntityData<FrostySdk.Ebx.WSClearAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.WSClearAreaTriggerEntityData Data => data as FrostySdk.Ebx.WSClearAreaTriggerEntityData;

		public WSClearAreaTriggerEntity(FrostySdk.Ebx.WSClearAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

