using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraModifierEntityData))]
	public class CameraModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CameraModifierEntityData>
	{
		public new FrostySdk.Ebx.CameraModifierEntityData Data => data as FrostySdk.Ebx.CameraModifierEntityData;
		public override string DisplayName => "CameraModifier";

		public CameraModifierEntity(FrostySdk.Ebx.CameraModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

