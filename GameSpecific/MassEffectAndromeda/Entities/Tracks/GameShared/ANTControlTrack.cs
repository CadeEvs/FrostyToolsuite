
using LevelEditorPlugin.Editors;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTControlTrackData))]
	public class ANTControlTrack : LinkTrack, IEntityData<FrostySdk.Ebx.ANTControlTrackData>
	{
		public new FrostySdk.Ebx.ANTControlTrackData Data => data as FrostySdk.Ebx.ANTControlTrackData;
		public override string DisplayName => "ANTControlTrack";
        public override string Icon => "Images/Tracks/AntTrack.png";

        public ANTControlTrack(FrostySdk.Ebx.ANTControlTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			var evalData = Data.EvalData.GetObjectAs<FrostySdk.Ebx.ANTEvaluatorData>();
			if (evalData != null)
			{
				foreach (var objRef in evalData.LayerTracks)
				{
					AddTrack(objRef);
				}
			}
		}
	}
}

