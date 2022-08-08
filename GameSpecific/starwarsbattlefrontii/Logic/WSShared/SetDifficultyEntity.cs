using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetDifficultyEntityData))]
	public class SetDifficultyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetDifficultyEntityData>
	{
		public new FrostySdk.Ebx.SetDifficultyEntityData Data => data as FrostySdk.Ebx.SetDifficultyEntityData;
		public override string DisplayName => "SetDifficulty";

		public SetDifficultyEntity(FrostySdk.Ebx.SetDifficultyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

