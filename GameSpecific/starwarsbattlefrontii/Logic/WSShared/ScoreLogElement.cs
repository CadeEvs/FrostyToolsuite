using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScoreLogElementData))]
	public class ScoreLogElement : GameEventsListElementBase, IEntityData<FrostySdk.Ebx.ScoreLogElementData>
	{
		public new FrostySdk.Ebx.ScoreLogElementData Data => data as FrostySdk.Ebx.ScoreLogElementData;
		public override string DisplayName => "ScoreLogElement";

		public ScoreLogElement(FrostySdk.Ebx.ScoreLogElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

