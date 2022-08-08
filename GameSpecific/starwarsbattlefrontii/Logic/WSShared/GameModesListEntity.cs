using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameModesListEntityData))]
	public class GameModesListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameModesListEntityData>
	{
		public new FrostySdk.Ebx.GameModesListEntityData Data => data as FrostySdk.Ebx.GameModesListEntityData;
		public override string DisplayName => "GameModesList";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GameModesListEntity(FrostySdk.Ebx.GameModesListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

