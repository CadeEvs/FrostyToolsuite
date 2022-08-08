using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClothColliderControlEntityData))]
	public class ClothColliderControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClothColliderControlEntityData>
	{
		public new FrostySdk.Ebx.ClothColliderControlEntityData Data => data as FrostySdk.Ebx.ClothColliderControlEntityData;
		public override string DisplayName => "ClothColliderControl";

		public ClothColliderControlEntity(FrostySdk.Ebx.ClothColliderControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

