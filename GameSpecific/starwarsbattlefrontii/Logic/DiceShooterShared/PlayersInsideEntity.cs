using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayersInsideEntityData))]
	public class PlayersInsideEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayersInsideEntityData>
	{
		public new FrostySdk.Ebx.PlayersInsideEntityData Data => data as FrostySdk.Ebx.PlayersInsideEntityData;
		public override string DisplayName => "PlayersInside";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayersInsideEntity(FrostySdk.Ebx.PlayersInsideEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

