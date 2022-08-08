using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OriginAccountWebPageLoaderEntityData))]
	public class OriginAccountWebPageLoaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OriginAccountWebPageLoaderEntityData>
	{
		public new FrostySdk.Ebx.OriginAccountWebPageLoaderEntityData Data => data as FrostySdk.Ebx.OriginAccountWebPageLoaderEntityData;
		public override string DisplayName => "OriginAccountWebPageLoader";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OriginAccountWebPageLoaderEntity(FrostySdk.Ebx.OriginAccountWebPageLoaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

