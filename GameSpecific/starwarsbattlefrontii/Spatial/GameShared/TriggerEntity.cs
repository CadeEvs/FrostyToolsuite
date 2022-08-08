using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TriggerEntityData))]
	public class TriggerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.TriggerEntityData>
	{
		public new FrostySdk.Ebx.TriggerEntityData Data => data as FrostySdk.Ebx.TriggerEntityData;

		public TriggerEntity(FrostySdk.Ebx.TriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

