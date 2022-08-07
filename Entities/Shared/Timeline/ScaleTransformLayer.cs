using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScaleTransformLayerData))]
	public class ScaleTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.ScaleTransformLayerData>
	{
		public new FrostySdk.Ebx.ScaleTransformLayerData Data => data as FrostySdk.Ebx.ScaleTransformLayerData;
		public override string DisplayName => "Scale Layer";

		protected FloatTrack scaleX;
		protected FloatTrack scaleY;
		protected FloatTrack scaleZ;

		public ScaleTransformLayer(FrostySdk.Ebx.ScaleTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.ScaleX, out scaleX);
			AddTrack(Data.ScaleY, out scaleY);
			AddTrack(Data.ScaleZ, out scaleZ);
		}

		public override void Update(float elapsedTime)
		{
			foreach (var track in tracks)
			{
				track.Update(elapsedTime);
			}

			currentValue = Matrix.Scaling((float)scaleX.CurrentValue, (float)scaleY.CurrentValue, (float)scaleZ.CurrentValue);
		}
	}
}

