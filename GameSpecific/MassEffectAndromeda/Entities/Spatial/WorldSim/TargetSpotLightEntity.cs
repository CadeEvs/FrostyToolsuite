using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetSpotLightEntityData))]
	public class TargetSpotLightEntity : SpotLightEntity, IEntityData<FrostySdk.Ebx.TargetSpotLightEntityData>
	{
		public new FrostySdk.Ebx.TargetSpotLightEntityData Data => data as FrostySdk.Ebx.TargetSpotLightEntityData;

		public TargetSpotLightEntity(FrostySdk.Ebx.TargetSpotLightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

