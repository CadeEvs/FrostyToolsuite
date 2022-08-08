using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DataSourceProviderCharacterStatData))]
	public class DataSourceProviderCharacterStat : LogicEntity, IEntityData<FrostySdk.Ebx.DataSourceProviderCharacterStatData>
	{
		public new FrostySdk.Ebx.DataSourceProviderCharacterStatData Data => data as FrostySdk.Ebx.DataSourceProviderCharacterStatData;
		public override string DisplayName => "DataSourceProviderCharacterStat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DataSourceProviderCharacterStat(FrostySdk.Ebx.DataSourceProviderCharacterStatData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

