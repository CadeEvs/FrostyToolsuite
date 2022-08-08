using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FixedTargetComposerData))]
	public class FixedTargetComposer : CinebotComposer, IEntityData<FrostySdk.Ebx.FixedTargetComposerData>
	{
		public new FrostySdk.Ebx.FixedTargetComposerData Data => data as FrostySdk.Ebx.FixedTargetComposerData;
		public override string DisplayName => "FixedTargetComposer";

		public FixedTargetComposer(FrostySdk.Ebx.FixedTargetComposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

