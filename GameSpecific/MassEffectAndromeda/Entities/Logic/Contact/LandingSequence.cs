using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LandingSequenceData))]
	public class LandingSequence : LogicEntity, IEntityData<FrostySdk.Ebx.LandingSequenceData>
	{
		public new FrostySdk.Ebx.LandingSequenceData Data => data as FrostySdk.Ebx.LandingSequenceData;
		public override string DisplayName => "LandingSequence";

		public LandingSequence(FrostySdk.Ebx.LandingSequenceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

