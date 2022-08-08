using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameModesFilterEntityData))]
	public class GameModesFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameModesFilterEntityData>
	{
		public new FrostySdk.Ebx.GameModesFilterEntityData Data => data as FrostySdk.Ebx.GameModesFilterEntityData;
		public override string DisplayName => "GameModesFilter";

		public GameModesFilterEntity(FrostySdk.Ebx.GameModesFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

