using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterInteractWaveEntityData))]
	public class WaterInteractWaveEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.WaterInteractWaveEntityData>
	{
		public new FrostySdk.Ebx.WaterInteractWaveEntityData Data => data as FrostySdk.Ebx.WaterInteractWaveEntityData;

		public WaterInteractWaveEntity(FrostySdk.Ebx.WaterInteractWaveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

