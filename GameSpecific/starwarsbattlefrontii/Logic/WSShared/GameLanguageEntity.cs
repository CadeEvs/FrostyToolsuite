using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameLanguageEntityData))]
	public class GameLanguageEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameLanguageEntityData>
	{
		public new FrostySdk.Ebx.GameLanguageEntityData Data => data as FrostySdk.Ebx.GameLanguageEntityData;
		public override string DisplayName => "GameLanguage";

		public GameLanguageEntity(FrostySdk.Ebx.GameLanguageEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

