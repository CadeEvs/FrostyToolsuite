using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingComposerData))]
	public class TargetingComposer : CinebotComposer, IEntityData<FrostySdk.Ebx.TargetingComposerData>
	{
		public new FrostySdk.Ebx.TargetingComposerData Data => data as FrostySdk.Ebx.TargetingComposerData;
		public override string DisplayName => "TargetingComposer";

		public TargetingComposer(FrostySdk.Ebx.TargetingComposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

