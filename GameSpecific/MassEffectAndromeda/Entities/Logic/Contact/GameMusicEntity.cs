using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameMusicEntityData))]
	public class GameMusicEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameMusicEntityData>
	{
		public new FrostySdk.Ebx.GameMusicEntityData Data => data as FrostySdk.Ebx.GameMusicEntityData;
		public override string DisplayName => "GameMusic";

		public GameMusicEntity(FrostySdk.Ebx.GameMusicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

