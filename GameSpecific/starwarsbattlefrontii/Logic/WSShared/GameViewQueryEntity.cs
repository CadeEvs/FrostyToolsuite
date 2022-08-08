using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameViewQueryEntityData))]
	public class GameViewQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameViewQueryEntityData>
	{
		public new FrostySdk.Ebx.GameViewQueryEntityData Data => data as FrostySdk.Ebx.GameViewQueryEntityData;
		public override string DisplayName => "GameViewQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GameViewQueryEntity(FrostySdk.Ebx.GameViewQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

