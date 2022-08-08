using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanetExitPositionerData))]
	public class PlanetExitPositioner : LogicEntity, IEntityData<FrostySdk.Ebx.PlanetExitPositionerData>
	{
		public new FrostySdk.Ebx.PlanetExitPositionerData Data => data as FrostySdk.Ebx.PlanetExitPositionerData;
		public override string DisplayName => "PlanetExitPositioner";

		public PlanetExitPositioner(FrostySdk.Ebx.PlanetExitPositionerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

