using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageIndicatorDataProviderData))]
	public class DamageIndicatorDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.DamageIndicatorDataProviderData>
	{
		public new FrostySdk.Ebx.DamageIndicatorDataProviderData Data => data as FrostySdk.Ebx.DamageIndicatorDataProviderData;
		public override string DisplayName => "DamageIndicatorDataProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DamageIndicatorDataProvider(FrostySdk.Ebx.DamageIndicatorDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

