using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AmbientEnvironmentEntityData))]
	public class AmbientEnvironmentEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AmbientEnvironmentEntityData>
	{
		public new FrostySdk.Ebx.AmbientEnvironmentEntityData Data => data as FrostySdk.Ebx.AmbientEnvironmentEntityData;
		public override string DisplayName => "AmbientEnvironment";

		public AmbientEnvironmentEntity(FrostySdk.Ebx.AmbientEnvironmentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

