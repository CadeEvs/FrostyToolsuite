using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayersQueryEntityData))]
	public class PlayersQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayersQueryEntityData>
	{
		public new FrostySdk.Ebx.PlayersQueryEntityData Data => data as FrostySdk.Ebx.PlayersQueryEntityData;
		public override string DisplayName => "PlayersQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayersQueryEntity(FrostySdk.Ebx.PlayersQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

