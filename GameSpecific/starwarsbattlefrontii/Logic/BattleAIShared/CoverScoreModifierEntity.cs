using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverScoreModifierEntityData))]
	public class CoverScoreModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoverScoreModifierEntityData>
	{
		public new FrostySdk.Ebx.CoverScoreModifierEntityData Data => data as FrostySdk.Ebx.CoverScoreModifierEntityData;
		public override string DisplayName => "CoverScoreModifier";

		public CoverScoreModifierEntity(FrostySdk.Ebx.CoverScoreModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

