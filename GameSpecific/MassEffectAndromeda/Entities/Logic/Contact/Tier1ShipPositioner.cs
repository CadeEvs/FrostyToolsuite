using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Tier1ShipPositionerData))]
	public class Tier1ShipPositioner : LogicEntity, IEntityData<FrostySdk.Ebx.Tier1ShipPositionerData>
	{
		public new FrostySdk.Ebx.Tier1ShipPositionerData Data => data as FrostySdk.Ebx.Tier1ShipPositionerData;
		public override string DisplayName => "Tier1ShipPositioner";

		public Tier1ShipPositioner(FrostySdk.Ebx.Tier1ShipPositionerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

