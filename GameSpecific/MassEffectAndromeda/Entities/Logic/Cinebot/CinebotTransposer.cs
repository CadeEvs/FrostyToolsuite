using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotTransposerData))]
	public class CinebotTransposer : CinebotTrackable, IEntityData<FrostySdk.Ebx.CinebotTransposerData>
	{
		public new FrostySdk.Ebx.CinebotTransposerData Data => data as FrostySdk.Ebx.CinebotTransposerData;
		public override string DisplayName => "CinebotTransposer";

		public CinebotTransposer(FrostySdk.Ebx.CinebotTransposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

