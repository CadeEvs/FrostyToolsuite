using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AerialViewpointEntityData))]
	public class AerialViewpointEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AerialViewpointEntityData>
	{
		public new FrostySdk.Ebx.AerialViewpointEntityData Data => data as FrostySdk.Ebx.AerialViewpointEntityData;

		public AerialViewpointEntity(FrostySdk.Ebx.AerialViewpointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

