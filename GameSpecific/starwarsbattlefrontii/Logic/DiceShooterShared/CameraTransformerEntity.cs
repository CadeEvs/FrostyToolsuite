using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraTransformerEntityData))]
	public class CameraTransformerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.CameraTransformerEntityData Data => data as FrostySdk.Ebx.CameraTransformerEntityData;
		public override string DisplayName => "CameraTransformer";

		public CameraTransformerEntity(FrostySdk.Ebx.CameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

