using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarZoneEntityData))]
	public class RadarZoneEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadarZoneEntityData>
	{
		public new FrostySdk.Ebx.RadarZoneEntityData Data => data as FrostySdk.Ebx.RadarZoneEntityData;
		public override string DisplayName => "RadarZone";

		public RadarZoneEntity(FrostySdk.Ebx.RadarZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

