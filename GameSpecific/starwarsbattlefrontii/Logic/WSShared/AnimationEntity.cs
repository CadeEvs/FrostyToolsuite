using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationEntityData))]
	public class AnimationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnimationEntityData>
	{
		public new FrostySdk.Ebx.AnimationEntityData Data => data as FrostySdk.Ebx.AnimationEntityData;
		public override string DisplayName => "Animation";

		public AnimationEntity(FrostySdk.Ebx.AnimationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

