using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterEquipmentBroadcasterData))]
	public class CharacterEquipmentBroadcaster : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterEquipmentBroadcasterData>
	{
		public new FrostySdk.Ebx.CharacterEquipmentBroadcasterData Data => data as FrostySdk.Ebx.CharacterEquipmentBroadcasterData;
		public override string DisplayName => "CharacterEquipmentBroadcaster";

		public CharacterEquipmentBroadcaster(FrostySdk.Ebx.CharacterEquipmentBroadcasterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

