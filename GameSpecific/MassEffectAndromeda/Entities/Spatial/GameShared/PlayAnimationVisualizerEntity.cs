using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayAnimationVisualizerEntityData))]
	public class PlayAnimationVisualizerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PlayAnimationVisualizerEntityData>
	{
		public new FrostySdk.Ebx.PlayAnimationVisualizerEntityData Data => data as FrostySdk.Ebx.PlayAnimationVisualizerEntityData;

		public PlayAnimationVisualizerEntity(FrostySdk.Ebx.PlayAnimationVisualizerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

