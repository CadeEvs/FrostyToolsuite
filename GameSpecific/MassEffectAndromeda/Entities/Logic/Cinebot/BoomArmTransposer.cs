using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoomArmTransposerData))]
	public class BoomArmTransposer : CinebotTransposer, IEntityData<FrostySdk.Ebx.BoomArmTransposerData>
	{
		public new FrostySdk.Ebx.BoomArmTransposerData Data => data as FrostySdk.Ebx.BoomArmTransposerData;
		public override string DisplayName => "BoomArmTransposer";

		public BoomArmTransposer(FrostySdk.Ebx.BoomArmTransposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

