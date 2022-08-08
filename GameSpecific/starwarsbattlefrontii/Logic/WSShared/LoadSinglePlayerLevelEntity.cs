using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadSinglePlayerLevelEntityData))]
	public class LoadSinglePlayerLevelEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LoadSinglePlayerLevelEntityData>
	{
		public new FrostySdk.Ebx.LoadSinglePlayerLevelEntityData Data => data as FrostySdk.Ebx.LoadSinglePlayerLevelEntityData;
		public override string DisplayName => "LoadSinglePlayerLevel";

		public LoadSinglePlayerLevelEntity(FrostySdk.Ebx.LoadSinglePlayerLevelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

