using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CurrentPlayerInfoEntityData))]
	public class CurrentPlayerInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CurrentPlayerInfoEntityData>
	{
		public new FrostySdk.Ebx.CurrentPlayerInfoEntityData Data => data as FrostySdk.Ebx.CurrentPlayerInfoEntityData;
		public override string DisplayName => "CurrentPlayerInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CurrentPlayerInfoEntity(FrostySdk.Ebx.CurrentPlayerInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

