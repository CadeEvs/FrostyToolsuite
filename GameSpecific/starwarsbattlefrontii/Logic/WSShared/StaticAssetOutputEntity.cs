using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticAssetOutputEntityData))]
	public class StaticAssetOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StaticAssetOutputEntityData>
	{
		public new FrostySdk.Ebx.StaticAssetOutputEntityData Data => data as FrostySdk.Ebx.StaticAssetOutputEntityData;
		public override string DisplayName => "StaticAssetOutput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StaticAssetOutputEntity(FrostySdk.Ebx.StaticAssetOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

