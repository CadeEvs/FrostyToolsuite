using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LensFlareEntityData))]
	public class LensFlareEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LensFlareEntityData>
	{
		public new FrostySdk.Ebx.LensFlareEntityData Data => data as FrostySdk.Ebx.LensFlareEntityData;

		public LensFlareEntity(FrostySdk.Ebx.LensFlareEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

