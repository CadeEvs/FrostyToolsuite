
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SchematicPinTrackData))]
	public class SchematicPinTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.SchematicPinTrackData>, ITimelineCustomTrackName
	{
		public new FrostySdk.Ebx.SchematicPinTrackData Data => data as FrostySdk.Ebx.SchematicPinTrackData;
		public override string DisplayName => "SchematicPinTrack";
		string ITimelineCustomTrackName.DisplayName => trackName;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				if (Data.ExposePins)
				{
					if (Data.TargetPinId != 0)
					{
						outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(Data.TargetPinId), Direction = Direction.In });
					}
					if (Data.SourcePinId != 0)
					{
						outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(Data.SourcePinId), Direction = Direction.Out });
					}
				}
				return outProperties;
			}
        }

		protected string trackName;

        public SchematicPinTrack(FrostySdk.Ebx.SchematicPinTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			trackName = FrostySdk.Utils.GetString(Data.TargetPinNameHash);
		}
	}
}

