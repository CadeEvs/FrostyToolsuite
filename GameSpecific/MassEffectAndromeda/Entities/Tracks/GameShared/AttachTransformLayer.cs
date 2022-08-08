
using LevelEditorPlugin.Editors;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AttachTransformLayerData))]
	public class AttachTransformLayer : TransformLayer, IEntityData<FrostySdk.Ebx.AttachTransformLayerData>, ITimelineCustomTrackName, ITimelineEntityProviderTrack
	{
		public new FrostySdk.Ebx.AttachTransformLayerData Data => data as FrostySdk.Ebx.AttachTransformLayerData;
		public Entity Entity => (attachToEntity is EntityTrackBase) ? (attachToEntity as EntityTrackBase).EntityLink : attachToEntity;
        public override string Icon => "Images/Tracks/EntityTrack.png";
        public override string DisplayName => "Attach Layer";
		string ITimelineCustomTrackName.DisplayName
        {
			get
            {
				if (attachToEntity == null)
					return "Obj: (null)";
				return $"Obj: {((attachToEntity is ITimelineCustomTrackName) ? (attachToEntity as ITimelineCustomTrackName).DisplayName : attachToEntity.DisplayName)}";
			}
        }

		protected Entity attachToEntity;

		public AttachTransformLayer(FrostySdk.Ebx.AttachTransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}

        public override void Initialize(ReferenceObject layerEntity)
        {
			base.Initialize(layerEntity);

			if (Data.AttachEntity.Type != FrostySdk.IO.PointerRefType.Null)
			{
				var attachTrack = layerEntity.FindEntity(Data.AttachEntity.GetInstanceGuid());
				if (attachTrack != null)
				{
					attachToEntity = attachTrack.FindAncestor<EntityTrackBase>();
				}
			}
			else if (Data.AttachEntityGuidChain.Count > 0)
			{
				attachToEntity = layerEntity.FindEntity(Data.AttachEntityGuidChain[0]);
			}
        }
    }
}

