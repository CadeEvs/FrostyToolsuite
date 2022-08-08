using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AreaTriggerEntityData))]
	public class AreaTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.AreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.AreaTriggerEntityData Data => data as FrostySdk.Ebx.AreaTriggerEntityData;

		public AreaTriggerEntity(FrostySdk.Ebx.AreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

