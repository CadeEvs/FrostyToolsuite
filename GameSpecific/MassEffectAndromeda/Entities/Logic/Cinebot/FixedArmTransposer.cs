using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FixedArmTransposerData))]
	public class FixedArmTransposer : CinebotTransposer, IEntityData<FrostySdk.Ebx.FixedArmTransposerData>
	{
		public new FrostySdk.Ebx.FixedArmTransposerData Data => data as FrostySdk.Ebx.FixedArmTransposerData;
		public override string DisplayName => "FixedArmTransposer";

		public FixedArmTransposer(FrostySdk.Ebx.FixedArmTransposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

