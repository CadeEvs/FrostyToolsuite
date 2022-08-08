using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanetIdCollectionEntityData))]
	public class PlanetIdCollectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlanetIdCollectionEntityData>
	{
		public new FrostySdk.Ebx.PlanetIdCollectionEntityData Data => data as FrostySdk.Ebx.PlanetIdCollectionEntityData;
		public override string DisplayName => "PlanetIdCollection";

		public PlanetIdCollectionEntity(FrostySdk.Ebx.PlanetIdCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

