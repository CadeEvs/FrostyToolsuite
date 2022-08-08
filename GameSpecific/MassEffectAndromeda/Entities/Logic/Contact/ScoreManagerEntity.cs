using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreManagerEntityData))]
	public class ScoreManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScoreManagerEntityData>
	{
		public new FrostySdk.Ebx.ScoreManagerEntityData Data => data as FrostySdk.Ebx.ScoreManagerEntityData;
		public override string DisplayName => "ScoreManager";

		public ScoreManagerEntity(FrostySdk.Ebx.ScoreManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

