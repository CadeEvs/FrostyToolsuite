using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CSMStateProviderEntityData))]
	public class CSMStateProviderEntity : ProviderEntity, IEntityData<FrostySdk.Ebx.CSMStateProviderEntityData>
	{
		public new FrostySdk.Ebx.CSMStateProviderEntityData Data => data as FrostySdk.Ebx.CSMStateProviderEntityData;
		public override string DisplayName => "CSMStateProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CSMStateProviderEntity(FrostySdk.Ebx.CSMStateProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

