using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UISimpleListDataProviderData))]
	public class UISimpleListDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.UISimpleListDataProviderData>
	{
		public new FrostySdk.Ebx.UISimpleListDataProviderData Data => data as FrostySdk.Ebx.UISimpleListDataProviderData;
		public override string DisplayName => "UISimpleListDataProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public UISimpleListDataProvider(FrostySdk.Ebx.UISimpleListDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

