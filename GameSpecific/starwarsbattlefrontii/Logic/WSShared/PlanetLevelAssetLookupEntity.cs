using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlanetLevelAssetLookupEntityData))]
	public class PlanetLevelAssetLookupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlanetLevelAssetLookupEntityData>
	{
		public new FrostySdk.Ebx.PlanetLevelAssetLookupEntityData Data => data as FrostySdk.Ebx.PlanetLevelAssetLookupEntityData;
		public override string DisplayName => "PlanetLevelAssetLookup";

		public PlanetLevelAssetLookupEntity(FrostySdk.Ebx.PlanetLevelAssetLookupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

