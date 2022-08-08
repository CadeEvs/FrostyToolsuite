using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestEffectEntityData))]
	public class TestEffectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TestEffectEntityData>
	{
		public new FrostySdk.Ebx.TestEffectEntityData Data => data as FrostySdk.Ebx.TestEffectEntityData;
		public override string DisplayName => "TestEffect";

		public TestEffectEntity(FrostySdk.Ebx.TestEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

