using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalLightEntityData))]
	public class LocalLightEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LocalLightEntityData>
	{
		public new FrostySdk.Ebx.LocalLightEntityData Data => data as FrostySdk.Ebx.LocalLightEntityData;

		public LocalLightEntity(FrostySdk.Ebx.LocalLightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

