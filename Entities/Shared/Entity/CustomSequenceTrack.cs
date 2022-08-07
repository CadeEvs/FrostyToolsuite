using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomSequenceTrackData))]
	public class CustomSequenceTrack : LogicEntity, IEntityData<FrostySdk.Ebx.CustomSequenceTrackData>
	{
		public new FrostySdk.Ebx.CustomSequenceTrackData Data => data as FrostySdk.Ebx.CustomSequenceTrackData;
		public override string DisplayName => "CustomSequenceTrack";

		public CustomSequenceTrack(FrostySdk.Ebx.CustomSequenceTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

