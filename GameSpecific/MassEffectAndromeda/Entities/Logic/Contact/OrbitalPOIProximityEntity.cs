using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OrbitalPOIProximityEntityData))]
	public class OrbitalPOIProximityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OrbitalPOIProximityEntityData>
	{
		public new FrostySdk.Ebx.OrbitalPOIProximityEntityData Data => data as FrostySdk.Ebx.OrbitalPOIProximityEntityData;
		public override string DisplayName => "OrbitalPOIProximity";

		public OrbitalPOIProximityEntity(FrostySdk.Ebx.OrbitalPOIProximityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

