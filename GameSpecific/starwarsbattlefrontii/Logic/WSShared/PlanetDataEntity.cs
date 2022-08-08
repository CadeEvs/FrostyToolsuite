using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanetDataEntityData))]
	public class PlanetDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlanetDataEntityData>
	{
		public new FrostySdk.Ebx.PlanetDataEntityData Data => data as FrostySdk.Ebx.PlanetDataEntityData;
		public override string DisplayName => "PlanetData";

		public PlanetDataEntity(FrostySdk.Ebx.PlanetDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

