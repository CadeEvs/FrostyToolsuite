using FrostySdk;
using LevelEditorPlugin.Editors;
using System.Collections.Generic;
using FrostySdk.Ebx;
using SharpDX;
using LinearTransform = FrostySdk.Ebx.LinearTransform;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LayeredTransformTrackData))]
	public class LayeredTransformTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.LayeredTransformTrackData>
	{
		public new FrostySdk.Ebx.LayeredTransformTrackData Data => data as FrostySdk.Ebx.LayeredTransformTrackData;
		public override string DisplayName => "LayeredTransformTrack";
		public override string Icon => "Images/Tracks/TransformTrack.png";
		public override object CurrentValue => currentValue;

		protected Matrix currentValue;

		protected Property<LinearTransform> sourceProperty;
		protected Property<LinearTransform> targetProperty;

		public LayeredTransformTrack(FrostySdk.Ebx.LayeredTransformTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			foreach (PointerRef objPointer in Data.LayerTracks)
			{
				TransformLayerData objectData = objPointer.GetObjectAs<FrostySdk.Ebx.TransformLayerData>();
				Entity track = CreateEntity(objectData);

				if (track != null)
				{
					tracks.Add(track as TimelineTrack);
				}
			}

			if (Data.ExposePins)
			{
				if (Data.SourcePinId != 0)
				{
					sourceProperty = new Property<LinearTransform>(owningTimeline, Data.SourcePinId, new LinearTransform());
				}
				if (Data.TargetPinId != 0)
				{
					targetProperty = new Property<LinearTransform>(owningTimeline, Data.TargetPinId, new LinearTransform());
				}
			}
		}

        public override void Update(float elapsedTime)
        {
			currentValue = Matrix.Identity;
            foreach (TimelineTrack track in tracks)
            {
				track.Update(elapsedTime);
				currentValue = currentValue * (track as TransformLayer).CurrentValue;
            }

			if (sourceProperty != null)
			{
				sourceProperty.Value = MakeLinearTransform(currentValue);
			}
		}
	}
}

