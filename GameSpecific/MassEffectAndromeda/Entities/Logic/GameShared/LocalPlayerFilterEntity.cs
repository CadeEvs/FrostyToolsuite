using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerFilterEntityData))]
	public class LocalPlayerFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerFilterEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerFilterEntityData Data => data as FrostySdk.Ebx.LocalPlayerFilterEntityData;
		public override string DisplayName => "LocalPlayerFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocalPlayerFilterEntity(FrostySdk.Ebx.LocalPlayerFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

