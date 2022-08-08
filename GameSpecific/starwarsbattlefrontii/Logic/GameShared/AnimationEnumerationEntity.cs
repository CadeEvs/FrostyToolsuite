using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationEnumerationEntityData))]
	public class AnimationEnumerationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnimationEnumerationEntityData>
	{
		public new FrostySdk.Ebx.AnimationEnumerationEntityData Data => data as FrostySdk.Ebx.AnimationEnumerationEntityData;
		public override string DisplayName => "AnimationEnumeration";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AnimationEnumerationEntity(FrostySdk.Ebx.AnimationEnumerationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

