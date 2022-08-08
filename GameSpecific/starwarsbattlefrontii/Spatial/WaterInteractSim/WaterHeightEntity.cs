using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaterHeightEntityData))]
	public class WaterHeightEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.WaterHeightEntityData>
	{
		public new FrostySdk.Ebx.WaterHeightEntityData Data => data as FrostySdk.Ebx.WaterHeightEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WaterHeightEntity(FrostySdk.Ebx.WaterHeightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

