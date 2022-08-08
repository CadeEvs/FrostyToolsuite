using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraLODEntityData))]
	public class CameraLODEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CameraLODEntityData>
	{
		public new FrostySdk.Ebx.CameraLODEntityData Data => data as FrostySdk.Ebx.CameraLODEntityData;
		public override string DisplayName => "CameraLOD";

		public CameraLODEntity(FrostySdk.Ebx.CameraLODEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

