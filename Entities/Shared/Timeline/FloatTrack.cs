
using FrostySdk.Ebx;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Library;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatTrackData))]
	public class FloatTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.FloatTrackData>
	{
		public new FrostySdk.Ebx.FloatTrackData Data => data as FrostySdk.Ebx.FloatTrackData;
		public override string DisplayName => "FloatTrack";
		public override string Icon => "Images/Tracks/FloatTrack.png";
		public override object CurrentValue => currentValue;

		protected float currentValue;

		protected Property<float> sourceProperty;
		protected Property<float> targetProperty;

		public FloatTrack(FrostySdk.Ebx.FloatTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			if (Data.ExposePins)
			{
				if (Data.SourcePinId != 0)
				{
					sourceProperty = new Property<float>(owningTimeline, Data.SourcePinId, 0.0f);
				}
				if (Data.TargetPinId != 0)
				{
					targetProperty = new Property<float>(owningTimeline, Data.TargetPinId, 0.0f);
				}
			}
		}

        public override void Update(float elapsedTime)
        {
			CurveData curveData = Data.CurveData.GetObjectAs<FrostySdk.Ebx.CurveData>();
			if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Zero)
			{
				currentValue = 0.0f;
				return;
			}
			else if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_One)
			{
				currentValue = 1.0f;
				return;
			}
			else if (curveData.CurveType == FrostySdk.Ebx.CurveType.CurveType_Constant)
			{
				currentValue = curveData.Value[0];
				return;
			}
			else
			{
				int index = -1;
				for (int i = 0; i < curveData.Time.Count; i++)
				{
					if (elapsedTime >= curveData.Time[i])
					{
						index = i;
					}
				}

				if (index + 1 >= curveData.Time.Count)
				{
					currentValue = curveData.Value[index];
				}
				else if (index == -1)
				{
					currentValue = curveData.Value[0];
				}
				else
				{
					float prevValue = curveData.Value[index];
					float nextValue = curveData.Value[index + 1];
					float prevTime = curveData.Time[index];
					float nextTime = curveData.Time[index + 1];

					currentValue = SharpDX.MathUtil.Lerp(prevValue, nextValue, (elapsedTime - prevTime) / (nextTime - prevTime));
				}
			}

			if (sourceProperty != null)
			{
				sourceProperty.Value = currentValue;
			}
        }
	}
}

