using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIPointTargetData))]
	public class AIPointTarget : SpatialEntity, IEntityData<FrostySdk.Ebx.AIPointTargetData>
	{
		public new FrostySdk.Ebx.AIPointTargetData Data => data as FrostySdk.Ebx.AIPointTargetData;

		public AIPointTarget(FrostySdk.Ebx.AIPointTargetData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

