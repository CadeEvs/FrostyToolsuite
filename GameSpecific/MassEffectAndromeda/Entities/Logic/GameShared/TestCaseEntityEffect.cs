using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TestCaseEntityEffectData))]
	public class TestCaseEntityEffect : LogicEntity, IEntityData<FrostySdk.Ebx.TestCaseEntityEffectData>
	{
		public new FrostySdk.Ebx.TestCaseEntityEffectData Data => data as FrostySdk.Ebx.TestCaseEntityEffectData;
		public override string DisplayName => "TestCaseEntityEffect";

		public TestCaseEntityEffect(FrostySdk.Ebx.TestCaseEntityEffectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

