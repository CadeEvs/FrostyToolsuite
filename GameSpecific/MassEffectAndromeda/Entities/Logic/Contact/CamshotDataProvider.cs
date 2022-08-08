using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CamshotDataProviderData))]
	public class CamshotDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.CamshotDataProviderData>
	{
		public new FrostySdk.Ebx.CamshotDataProviderData Data => data as FrostySdk.Ebx.CamshotDataProviderData;
		public override string DisplayName => "CamshotDataProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CamshotDataProvider(FrostySdk.Ebx.CamshotDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

