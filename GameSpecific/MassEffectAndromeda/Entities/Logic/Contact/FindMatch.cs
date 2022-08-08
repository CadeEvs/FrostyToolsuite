using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FindMatchData))]
	public class FindMatch : LogicEntity, IEntityData<FrostySdk.Ebx.FindMatchData>
	{
		public new FrostySdk.Ebx.FindMatchData Data => data as FrostySdk.Ebx.FindMatchData;
		public override string DisplayName => "FindMatch";

		public FindMatch(FrostySdk.Ebx.FindMatchData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

