using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MatchControlData))]
	public class MatchControl : LogicEntity, IEntityData<FrostySdk.Ebx.MatchControlData>
	{
		public new FrostySdk.Ebx.MatchControlData Data => data as FrostySdk.Ebx.MatchControlData;
		public override string DisplayName => "MatchControl";

		public MatchControl(FrostySdk.Ebx.MatchControlData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

