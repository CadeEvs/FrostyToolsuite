using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSIsSpectateBlockedEntityData))]
	public class WSIsSpectateBlockedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSIsSpectateBlockedEntityData>
	{
		public new FrostySdk.Ebx.WSIsSpectateBlockedEntityData Data => data as FrostySdk.Ebx.WSIsSpectateBlockedEntityData;
		public override string DisplayName => "WSIsSpectateBlocked";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSIsSpectateBlockedEntity(FrostySdk.Ebx.WSIsSpectateBlockedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

