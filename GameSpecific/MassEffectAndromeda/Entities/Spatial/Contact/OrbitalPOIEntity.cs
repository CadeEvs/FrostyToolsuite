using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OrbitalPOIEntityData))]
	public class OrbitalPOIEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.OrbitalPOIEntityData>
	{
		public new FrostySdk.Ebx.OrbitalPOIEntityData Data => data as FrostySdk.Ebx.OrbitalPOIEntityData;

		public OrbitalPOIEntity(FrostySdk.Ebx.OrbitalPOIEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

