using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetOrderedTargetEntityData))]
	public class SetOrderedTargetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetOrderedTargetEntityData>
	{
		public new FrostySdk.Ebx.SetOrderedTargetEntityData Data => data as FrostySdk.Ebx.SetOrderedTargetEntityData;
		public override string DisplayName => "SetOrderedTarget";

		public SetOrderedTargetEntity(FrostySdk.Ebx.SetOrderedTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

