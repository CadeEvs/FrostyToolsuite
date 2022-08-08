
using LevelEditorPlugin.Editors;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTEvalTrackData))]
	public class ANTEvalTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.ANTEvalTrackData>
	{
		public new FrostySdk.Ebx.ANTEvalTrackData Data => data as FrostySdk.Ebx.ANTEvalTrackData;
		public override string DisplayName => "ANTEvalTrack";
		public override string Icon => "Images/Tracks/AntTrack.png";

		public ANTEvalTrack(FrostySdk.Ebx.ANTEvalTrackData inData, Entity inParent)
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

