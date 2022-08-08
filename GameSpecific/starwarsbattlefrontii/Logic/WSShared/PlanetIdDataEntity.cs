using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanetIdDataEntityData))]
	public class PlanetIdDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlanetIdDataEntityData>
	{
		public new FrostySdk.Ebx.PlanetIdDataEntityData Data => data as FrostySdk.Ebx.PlanetIdDataEntityData;
		public override string DisplayName => "PlanetIdData";

		public PlanetIdDataEntity(FrostySdk.Ebx.PlanetIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

