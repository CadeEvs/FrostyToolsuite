using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimatableCullingEntityData))]
	public class AnimatableCullingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnimatableCullingEntityData>
	{
		public new FrostySdk.Ebx.AnimatableCullingEntityData Data => data as FrostySdk.Ebx.AnimatableCullingEntityData;
		public override string DisplayName => "AnimatableCulling";

		public AnimatableCullingEntity(FrostySdk.Ebx.AnimatableCullingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

