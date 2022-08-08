using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceSphereEntityData))]
	public class LocalWindForceSphereEntity : LocalWindForceEntityBase, IEntityData<FrostySdk.Ebx.LocalWindForceSphereEntityData>
	{
		public new FrostySdk.Ebx.LocalWindForceSphereEntityData Data => data as FrostySdk.Ebx.LocalWindForceSphereEntityData;

		public LocalWindForceSphereEntity(FrostySdk.Ebx.LocalWindForceSphereEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

