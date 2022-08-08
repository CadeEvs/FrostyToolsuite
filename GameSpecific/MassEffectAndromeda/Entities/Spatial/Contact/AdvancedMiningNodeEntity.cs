using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AdvancedMiningNodeEntityData))]
	public class AdvancedMiningNodeEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AdvancedMiningNodeEntityData>
	{
		public new FrostySdk.Ebx.AdvancedMiningNodeEntityData Data => data as FrostySdk.Ebx.AdvancedMiningNodeEntityData;

		public AdvancedMiningNodeEntity(FrostySdk.Ebx.AdvancedMiningNodeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

