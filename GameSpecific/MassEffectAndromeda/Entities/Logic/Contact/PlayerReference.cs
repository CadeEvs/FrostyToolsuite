using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerReferenceData))]
	public class PlayerReference : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerReferenceData>
	{
		public new FrostySdk.Ebx.PlayerReferenceData Data => data as FrostySdk.Ebx.PlayerReferenceData;
		public override string DisplayName => "PlayerReference";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerReference(FrostySdk.Ebx.PlayerReferenceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

