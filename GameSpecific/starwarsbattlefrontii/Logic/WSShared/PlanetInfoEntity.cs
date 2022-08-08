using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanetInfoEntityData))]
	public class PlanetInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlanetInfoEntityData>
	{
		public new FrostySdk.Ebx.PlanetInfoEntityData Data => data as FrostySdk.Ebx.PlanetInfoEntityData;
		public override string DisplayName => "PlanetInfo";

		public PlanetInfoEntity(FrostySdk.Ebx.PlanetInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

