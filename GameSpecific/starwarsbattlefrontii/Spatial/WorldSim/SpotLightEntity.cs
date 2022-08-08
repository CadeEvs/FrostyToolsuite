using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpotLightEntityData))]
	public class SpotLightEntity : LocalLightEntity, IEntityData<FrostySdk.Ebx.SpotLightEntityData>
	{
		public new FrostySdk.Ebx.SpotLightEntityData Data => data as FrostySdk.Ebx.SpotLightEntityData;

		public SpotLightEntity(FrostySdk.Ebx.SpotLightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

