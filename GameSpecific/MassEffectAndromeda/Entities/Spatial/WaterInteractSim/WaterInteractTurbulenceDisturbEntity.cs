using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterInteractTurbulenceDisturbEntityData))]
	public class WaterInteractTurbulenceDisturbEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.WaterInteractTurbulenceDisturbEntityData>
	{
		public new FrostySdk.Ebx.WaterInteractTurbulenceDisturbEntityData Data => data as FrostySdk.Ebx.WaterInteractTurbulenceDisturbEntityData;

		public WaterInteractTurbulenceDisturbEntity(FrostySdk.Ebx.WaterInteractTurbulenceDisturbEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

