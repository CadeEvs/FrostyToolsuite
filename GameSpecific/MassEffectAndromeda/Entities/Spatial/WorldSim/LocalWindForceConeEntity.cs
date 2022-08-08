using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceConeEntityData))]
	public class LocalWindForceConeEntity : LocalWindForceEntityBase, IEntityData<FrostySdk.Ebx.LocalWindForceConeEntityData>
	{
		public new FrostySdk.Ebx.LocalWindForceConeEntityData Data => data as FrostySdk.Ebx.LocalWindForceConeEntityData;

		public LocalWindForceConeEntity(FrostySdk.Ebx.LocalWindForceConeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

