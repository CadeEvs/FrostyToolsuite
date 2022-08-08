
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PathTransformLayerData))]
	public class PathTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.PathTransformLayerData>
	{
		public new FrostySdk.Ebx.PathTransformLayerData Data => data as FrostySdk.Ebx.PathTransformLayerData;
		public override string DisplayName => trackName;
        public override string Icon => "Images/Tracks/PathTrack.png";

        protected string trackName;
		protected FloatTrack timingCurveTrack;

		public PathTransformLayer(FrostySdk.Ebx.PathTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.TimingCurve, out timingCurveTrack);
		}

        public override void Initialize(ReferenceObject layerEntity)
        {
            base.Initialize(layerEntity);

			var entity = layerEntity.FindEntity(Data.PathEntityGuid);
			trackName = $"Path: {Data.PathEntityGuid}";

			if (entity != null)
			{
				trackName = $"Path: {entity.DisplayName}";
			}
        }
    }
}

