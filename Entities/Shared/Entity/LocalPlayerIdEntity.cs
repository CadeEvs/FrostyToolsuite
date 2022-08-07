using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerIdEntityData))]
	public class LocalPlayerIdEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerIdEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerIdEntityData Data => data as FrostySdk.Ebx.LocalPlayerIdEntityData;
		public override string DisplayName => "LocalPlayerId";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocalPlayerIdEntity(FrostySdk.Ebx.LocalPlayerIdEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

