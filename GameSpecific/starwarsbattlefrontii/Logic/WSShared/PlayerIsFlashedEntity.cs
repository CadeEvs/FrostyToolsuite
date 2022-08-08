using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerIsFlashedEntityData))]
	public class PlayerIsFlashedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerIsFlashedEntityData>
	{
		public new FrostySdk.Ebx.PlayerIsFlashedEntityData Data => data as FrostySdk.Ebx.PlayerIsFlashedEntityData;
		public override string DisplayName => "PlayerIsFlashed";

		public PlayerIsFlashedEntity(FrostySdk.Ebx.PlayerIsFlashedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

