using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotComposerData))]
	public class CinebotComposer : CinebotTrackable, IEntityData<FrostySdk.Ebx.CinebotComposerData>
	{
		public new FrostySdk.Ebx.CinebotComposerData Data => data as FrostySdk.Ebx.CinebotComposerData;
		public override string DisplayName => "CinebotComposer";

		public CinebotComposer(FrostySdk.Ebx.CinebotComposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

