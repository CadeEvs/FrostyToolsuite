using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexEntityData))]
	public class LocalWindForceBaked3DAs2x2DTexEntity : LocalWindForceEntityBase, IEntityData<FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexEntityData>
	{
		public new FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexEntityData Data => data as FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexEntityData;

		public LocalWindForceBaked3DAs2x2DTexEntity(FrostySdk.Ebx.LocalWindForceBaked3DAs2x2DTexEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

