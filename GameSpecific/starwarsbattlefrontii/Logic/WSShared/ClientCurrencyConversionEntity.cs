using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientCurrencyConversionEntityData))]
	public class ClientCurrencyConversionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientCurrencyConversionEntityData>
	{
		public new FrostySdk.Ebx.ClientCurrencyConversionEntityData Data => data as FrostySdk.Ebx.ClientCurrencyConversionEntityData;
		public override string DisplayName => "ClientCurrencyConversion";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientCurrencyConversionEntity(FrostySdk.Ebx.ClientCurrencyConversionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

