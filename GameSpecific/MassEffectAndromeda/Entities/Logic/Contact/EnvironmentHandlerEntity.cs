using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnvironmentHandlerEntityData))]
	public class EnvironmentHandlerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EnvironmentHandlerEntityData>
	{
		public new FrostySdk.Ebx.EnvironmentHandlerEntityData Data => data as FrostySdk.Ebx.EnvironmentHandlerEntityData;
		public override string DisplayName => "EnvironmentHandler";

		public EnvironmentHandlerEntity(FrostySdk.Ebx.EnvironmentHandlerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

