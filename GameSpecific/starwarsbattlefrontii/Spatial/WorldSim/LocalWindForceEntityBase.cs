using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceEntityBaseData))]
	public class LocalWindForceEntityBase : SpatialEntity, IEntityData<FrostySdk.Ebx.LocalWindForceEntityBaseData>
	{
		public new FrostySdk.Ebx.LocalWindForceEntityBaseData Data => data as FrostySdk.Ebx.LocalWindForceEntityBaseData;

		public LocalWindForceEntityBase(FrostySdk.Ebx.LocalWindForceEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

