using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerControllableLinksEntityData))]
	public class PlayerControllableLinksEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerControllableLinksEntityData>
	{
		public new FrostySdk.Ebx.PlayerControllableLinksEntityData Data => data as FrostySdk.Ebx.PlayerControllableLinksEntityData;
		public override string DisplayName => "PlayerControllableLinks";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerControllableLinksEntity(FrostySdk.Ebx.PlayerControllableLinksEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

