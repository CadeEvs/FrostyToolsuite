using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrAnalyticLightEntityData))]
	public class PbrAnalyticLightEntity : LocalLightEntity, IEntityData<FrostySdk.Ebx.PbrAnalyticLightEntityData>
	{
		public new FrostySdk.Ebx.PbrAnalyticLightEntityData Data => data as FrostySdk.Ebx.PbrAnalyticLightEntityData;

		public PbrAnalyticLightEntity(FrostySdk.Ebx.PbrAnalyticLightEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

