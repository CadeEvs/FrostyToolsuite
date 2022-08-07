
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoolTrackData))]
	public class BoolTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.BoolTrackData>
	{
		public new FrostySdk.Ebx.BoolTrackData Data => data as FrostySdk.Ebx.BoolTrackData;
		public override string DisplayName => "BoolTrack";
		public override string Icon => "Images/Tracks/BoolTrack.png";
        public override object CurrentValue => currentValue;

		protected bool currentValue;

        public BoolTrack(FrostySdk.Ebx.BoolTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			currentValue = true;
		}

        public override void Update(float elapsedTime)
        {
			if (Data.Keyframes.Count > 0)
			{
				currentValue = Data.Keyframes[0].Value;
				for (int i = Data.Keyframes.Count - 1; i >= 0; i--)
				{
					if (elapsedTime >= Data.Keyframes[i].Time)
					{
						currentValue = Data.Keyframes[i].Value;
						return;
					}
				}
			}
        }
    }
}

