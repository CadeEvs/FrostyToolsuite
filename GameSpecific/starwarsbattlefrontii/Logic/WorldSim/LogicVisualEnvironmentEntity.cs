using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogicVisualEnvironmentEntityData))]
	public class LogicVisualEnvironmentEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogicVisualEnvironmentEntityData>
	{
		public new FrostySdk.Ebx.LogicVisualEnvironmentEntityData Data => data as FrostySdk.Ebx.LogicVisualEnvironmentEntityData;
		public override string DisplayName => "LogicVisualEnvironment";

		public LogicVisualEnvironmentEntity(FrostySdk.Ebx.LogicVisualEnvironmentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

