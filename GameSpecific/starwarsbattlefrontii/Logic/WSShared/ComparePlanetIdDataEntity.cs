using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ComparePlanetIdDataEntityData))]
	public class ComparePlanetIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ComparePlanetIdDataEntityData>
	{
		public new FrostySdk.Ebx.ComparePlanetIdDataEntityData Data => data as FrostySdk.Ebx.ComparePlanetIdDataEntityData;
		public override string DisplayName => "ComparePlanetIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ComparePlanetIdDataEntity(FrostySdk.Ebx.ComparePlanetIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

