using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrTubeLightEntityData))]
	public class PbrTubeLightEntity : PbrAnalyticLightEntity, IEntityData<FrostySdk.Ebx.PbrTubeLightEntityData>
	{
		public new FrostySdk.Ebx.PbrTubeLightEntityData Data => data as FrostySdk.Ebx.PbrTubeLightEntityData;

		public PbrTubeLightEntity(FrostySdk.Ebx.PbrTubeLightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

