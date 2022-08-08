using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SamplerSplineEntityData))]
	public class SamplerSplineEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.SamplerSplineEntityData>
	{
		public new FrostySdk.Ebx.SamplerSplineEntityData Data => data as FrostySdk.Ebx.SamplerSplineEntityData;

		public SamplerSplineEntity(FrostySdk.Ebx.SamplerSplineEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

