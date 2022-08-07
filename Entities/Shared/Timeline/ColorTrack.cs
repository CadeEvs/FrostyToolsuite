using Vec4 = FrostySdk.Ebx.Vec4;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ColorTrackData))]
	public class ColorTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.ColorTrackData>
	{
		public new FrostySdk.Ebx.ColorTrackData Data => data as FrostySdk.Ebx.ColorTrackData;
		public override string DisplayName => "ColorTrack";
		public override string Icon => "Images/Tracks/ColorTrack.png";

		protected Vec4 currentValue;

		protected Property<Vec4> sourceProperty;
		protected Property<Vec4> targetProperty;

		public ColorTrack(FrostySdk.Ebx.ColorTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			if (Data.ExposePins)
			{
				if (Data.SourcePinId != 0)
				{
					sourceProperty = new Property<Vec4>(owningTimeline, Data.SourcePinId, new Vec4());
				}
				if (Data.TargetPinId != 0)
				{
					targetProperty = new Property<Vec4>(owningTimeline, Data.TargetPinId, new Vec4());
				}
			}
		}

		public override void Update(float elapsedTime)
		{
			int index = -1;
			for (int i = 0; i < Data.Keyframes.Count; i++)
			{
				if (elapsedTime >= Data.Keyframes[i].Time)
				{
					index = i;
				}
			}

			if (index + 1 >= Data.Keyframes.Count)
			{
				currentValue = Data.Keyframes[index].RGBColor;
			}
			else if (index == -1)
			{
				currentValue = Data.Keyframes[0].RGBColor;
			}
			else
			{
				Vec4 prevValue = Data.Keyframes[index].RGBColor;
				Vec4 nextValue = Data.Keyframes[index + 1].RGBColor;
				float prevTime = Data.Keyframes[index].Time;
				float nextTime = Data.Keyframes[index + 1].Time;

				currentValue = new Vec4();
				currentValue.x = SharpDX.MathUtil.Lerp(prevValue.x, nextValue.x, (elapsedTime - prevTime) / (nextTime - prevTime));
				currentValue.y = SharpDX.MathUtil.Lerp(prevValue.y, nextValue.y, (elapsedTime - prevTime) / (nextTime - prevTime));
				currentValue.z = SharpDX.MathUtil.Lerp(prevValue.z, nextValue.z, (elapsedTime - prevTime) / (nextTime - prevTime));
				currentValue.w = SharpDX.MathUtil.Lerp(prevValue.w, nextValue.w, (elapsedTime - prevTime) / (nextTime - prevTime));
			}

			if (sourceProperty != null)
			{
				sourceProperty.Value = currentValue;
			}
		}
	}
}

