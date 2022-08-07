using LevelEditorPlugin.Editors;
using System.Collections.Generic;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KeyedTransformLayerData))]
	public class KeyedTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.KeyedTransformLayerData>
	{
		public new FrostySdk.Ebx.KeyedTransformLayerData Data => data as FrostySdk.Ebx.KeyedTransformLayerData;
		public override string DisplayName => "Keyed Layer";

		protected FloatTrack TranslationX;
		protected FloatTrack TranslationY;
		protected FloatTrack TranslationZ;
		protected FloatTrack RotationX;
		protected FloatTrack RotationY;
		protected FloatTrack RotationZ;

		public KeyedTransformLayer(FrostySdk.Ebx.KeyedTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.TranslationX, out TranslationX);
			AddTrack(Data.TranslationY, out TranslationY);
			AddTrack(Data.TranslationZ, out TranslationZ);
			AddTrack(Data.RotationX, out RotationX);
			AddTrack(Data.RotationY, out RotationY);
			AddTrack(Data.RotationZ, out RotationZ);
		}

        public override void Update(float elapsedTime)
        {
            foreach (TimelineTrack track in tracks)
            {
				track.Update(elapsedTime);
            }

			currentValue = 
				Matrix.RotationX(MathUtil.DegreesToRadians((float)RotationX.CurrentValue)) *
				Matrix.RotationY(MathUtil.DegreesToRadians((float)RotationY.CurrentValue)) *
				Matrix.RotationZ(MathUtil.DegreesToRadians((float)RotationZ.CurrentValue)) *
				Matrix.Translation((float)TranslationX.CurrentValue, (float)TranslationY.CurrentValue, (float)TranslationZ.CurrentValue);
        }
    }
}

