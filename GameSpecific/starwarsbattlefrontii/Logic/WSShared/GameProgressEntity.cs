using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameProgressEntityData))]
	public class GameProgressEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameProgressEntityData>
	{
		public new FrostySdk.Ebx.GameProgressEntityData Data => data as FrostySdk.Ebx.GameProgressEntityData;
		public override string DisplayName => "GameProgress";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GameProgressEntity(FrostySdk.Ebx.GameProgressEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

