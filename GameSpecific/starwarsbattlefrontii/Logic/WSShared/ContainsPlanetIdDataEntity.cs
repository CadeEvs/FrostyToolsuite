using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContainsPlanetIdDataEntityData))]
	public class ContainsPlanetIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ContainsPlanetIdDataEntityData>
	{
		public new FrostySdk.Ebx.ContainsPlanetIdDataEntityData Data => data as FrostySdk.Ebx.ContainsPlanetIdDataEntityData;
		public override string DisplayName => "ContainsPlanetIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContainsPlanetIdDataEntity(FrostySdk.Ebx.ContainsPlanetIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

