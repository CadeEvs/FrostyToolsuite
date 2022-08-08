using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FatalEnvironmentEntityData))]
	public class FatalEnvironmentEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FatalEnvironmentEntityData>
	{
		public new FrostySdk.Ebx.FatalEnvironmentEntityData Data => data as FrostySdk.Ebx.FatalEnvironmentEntityData;
		public override string DisplayName => "FatalEnvironment";

		public FatalEnvironmentEntity(FrostySdk.Ebx.FatalEnvironmentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

