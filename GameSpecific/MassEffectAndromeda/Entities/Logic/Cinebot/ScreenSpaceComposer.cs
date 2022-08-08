using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScreenSpaceComposerData))]
	public class ScreenSpaceComposer : CinebotComposer, IEntityData<FrostySdk.Ebx.ScreenSpaceComposerData>
	{
		public new FrostySdk.Ebx.ScreenSpaceComposerData Data => data as FrostySdk.Ebx.ScreenSpaceComposerData;
		public override string DisplayName => "ScreenSpaceComposer";

		public ScreenSpaceComposer(FrostySdk.Ebx.ScreenSpaceComposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

