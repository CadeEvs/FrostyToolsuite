using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestCullIdCullableEffectEntityData))]
	public class TestCullIdCullableEffectEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.TestCullIdCullableEffectEntityData>
	{
		public new FrostySdk.Ebx.TestCullIdCullableEffectEntityData Data => data as FrostySdk.Ebx.TestCullIdCullableEffectEntityData;

		public TestCullIdCullableEffectEntity(FrostySdk.Ebx.TestCullIdCullableEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

