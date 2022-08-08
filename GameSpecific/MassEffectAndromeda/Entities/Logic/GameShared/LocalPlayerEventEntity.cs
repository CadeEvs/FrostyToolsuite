using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerEventEntityData))]
	public class LocalPlayerEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerEventEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerEventEntityData Data => data as FrostySdk.Ebx.LocalPlayerEventEntityData;
		public override string DisplayName => "LocalPlayerEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocalPlayerEventEntity(FrostySdk.Ebx.LocalPlayerEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

