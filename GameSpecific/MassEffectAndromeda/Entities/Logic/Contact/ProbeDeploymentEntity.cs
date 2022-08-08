using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProbeDeploymentEntityData))]
	public class ProbeDeploymentEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProbeDeploymentEntityData>
	{
		public new FrostySdk.Ebx.ProbeDeploymentEntityData Data => data as FrostySdk.Ebx.ProbeDeploymentEntityData;
		public override string DisplayName => "ProbeDeployment";

		public ProbeDeploymentEntity(FrostySdk.Ebx.ProbeDeploymentEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

