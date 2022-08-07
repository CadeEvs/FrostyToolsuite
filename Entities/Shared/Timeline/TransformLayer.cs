using LevelEditorPlugin.Editors;
using System.Collections.Generic;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformLayerData))]
	public class TransformLayer : TimelineTrack, IEntityData<FrostySdk.Ebx.TransformLayerData>
	{
		public new FrostySdk.Ebx.TransformLayerData Data => data as FrostySdk.Ebx.TransformLayerData;
		public override string DisplayName => "TransformLayer";
		public override string Icon => "Images/Tracks/LayerTrack.png";
		public Matrix CurrentValue => currentValue;

		protected Matrix currentValue;

		public TransformLayer(FrostySdk.Ebx.TransformLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.Weight);
		}
	}
}

