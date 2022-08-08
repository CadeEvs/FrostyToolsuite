using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsLocalPlayerEventEntityData))]
	public class IsLocalPlayerEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IsLocalPlayerEventEntityData>
	{
		public new FrostySdk.Ebx.IsLocalPlayerEventEntityData Data => data as FrostySdk.Ebx.IsLocalPlayerEventEntityData;
		public override string DisplayName => "IsLocalPlayerEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IsLocalPlayerEventEntity(FrostySdk.Ebx.IsLocalPlayerEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

