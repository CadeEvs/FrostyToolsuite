using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceSamplerEntityData))]
	public class LocalWindForceSamplerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LocalWindForceSamplerEntityData>
	{
		public new FrostySdk.Ebx.LocalWindForceSamplerEntityData Data => data as FrostySdk.Ebx.LocalWindForceSamplerEntityData;

		public LocalWindForceSamplerEntity(FrostySdk.Ebx.LocalWindForceSamplerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

