using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterDepthEntityData))]
	public class WaterDepthEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.WaterDepthEntityData>
	{
		public new FrostySdk.Ebx.WaterDepthEntityData Data => data as FrostySdk.Ebx.WaterDepthEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WaterDepthEntity(FrostySdk.Ebx.WaterDepthEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

