using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BuffTriggerEntityData))]
	public class BuffTriggerEntity : GeometryTriggerEntity, IEntityData<FrostySdk.Ebx.BuffTriggerEntityData>
	{
		public new FrostySdk.Ebx.BuffTriggerEntityData Data => data as FrostySdk.Ebx.BuffTriggerEntityData;

		public BuffTriggerEntity(FrostySdk.Ebx.BuffTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

