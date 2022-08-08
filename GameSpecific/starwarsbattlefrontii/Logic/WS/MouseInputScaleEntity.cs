using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MouseInputScaleEntityData))]
	public class MouseInputScaleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MouseInputScaleEntityData>
	{
		public new FrostySdk.Ebx.MouseInputScaleEntityData Data => data as FrostySdk.Ebx.MouseInputScaleEntityData;
		public override string DisplayName => "MouseInputScale";

		public MouseInputScaleEntity(FrostySdk.Ebx.MouseInputScaleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

