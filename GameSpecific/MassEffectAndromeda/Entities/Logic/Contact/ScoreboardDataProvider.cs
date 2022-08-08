using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreboardDataProviderData))]
	public class ScoreboardDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.ScoreboardDataProviderData>
	{
		public new FrostySdk.Ebx.ScoreboardDataProviderData Data => data as FrostySdk.Ebx.ScoreboardDataProviderData;
		public override string DisplayName => "ScoreboardDataProvider";

		public ScoreboardDataProvider(FrostySdk.Ebx.ScoreboardDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

