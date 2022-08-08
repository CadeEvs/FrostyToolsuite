using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterAnimationLodEntityData))]
	public class CharacterAnimationLodEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterAnimationLodEntityData>
	{
		public new FrostySdk.Ebx.CharacterAnimationLodEntityData Data => data as FrostySdk.Ebx.CharacterAnimationLodEntityData;
		public override string DisplayName => "CharacterAnimationLod";

		public CharacterAnimationLodEntity(FrostySdk.Ebx.CharacterAnimationLodEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

