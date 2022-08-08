using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MostValuablePlayersEntityData))]
	public class MostValuablePlayersEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MostValuablePlayersEntityData>
	{
		public new FrostySdk.Ebx.MostValuablePlayersEntityData Data => data as FrostySdk.Ebx.MostValuablePlayersEntityData;
		public override string DisplayName => "MostValuablePlayers";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MostValuablePlayersEntity(FrostySdk.Ebx.MostValuablePlayersEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

