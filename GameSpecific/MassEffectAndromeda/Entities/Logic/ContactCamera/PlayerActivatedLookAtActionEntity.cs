using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerActivatedLookAtActionEntityData))]
	public class PlayerActivatedLookAtActionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerActivatedLookAtActionEntityData>
	{
		public new FrostySdk.Ebx.PlayerActivatedLookAtActionEntityData Data => data as FrostySdk.Ebx.PlayerActivatedLookAtActionEntityData;
		public override string DisplayName => "PlayerActivatedLookAtAction";

		public PlayerActivatedLookAtActionEntity(FrostySdk.Ebx.PlayerActivatedLookAtActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

