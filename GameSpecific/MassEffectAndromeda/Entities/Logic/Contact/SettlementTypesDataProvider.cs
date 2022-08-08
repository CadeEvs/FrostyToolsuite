using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SettlementTypesDataProviderData))]
	public class SettlementTypesDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.SettlementTypesDataProviderData>
	{
		public new FrostySdk.Ebx.SettlementTypesDataProviderData Data => data as FrostySdk.Ebx.SettlementTypesDataProviderData;
		public override string DisplayName => "SettlementTypesDataProvider";

		public SettlementTypesDataProvider(FrostySdk.Ebx.SettlementTypesDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

