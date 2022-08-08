using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameplaySupplyPointsEntityData))]
	public class GameplaySupplyPointsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameplaySupplyPointsEntityData>
	{
		public new FrostySdk.Ebx.GameplaySupplyPointsEntityData Data => data as FrostySdk.Ebx.GameplaySupplyPointsEntityData;
		public override string DisplayName => "GameplaySupplyPoints";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GameplaySupplyPointsEntity(FrostySdk.Ebx.GameplaySupplyPointsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

