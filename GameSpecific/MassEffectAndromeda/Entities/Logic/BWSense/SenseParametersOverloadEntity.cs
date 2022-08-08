using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SenseParametersOverloadEntityData))]
	public class SenseParametersOverloadEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SenseParametersOverloadEntityData>
	{
		public new FrostySdk.Ebx.SenseParametersOverloadEntityData Data => data as FrostySdk.Ebx.SenseParametersOverloadEntityData;
		public override string DisplayName => "SenseParametersOverload";

		public SenseParametersOverloadEntity(FrostySdk.Ebx.SenseParametersOverloadEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

