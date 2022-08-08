using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AnimationEnumerationChoiceEntityData))]
	public class AnimationEnumerationChoiceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AnimationEnumerationChoiceEntityData>
	{
		public new FrostySdk.Ebx.AnimationEnumerationChoiceEntityData Data => data as FrostySdk.Ebx.AnimationEnumerationChoiceEntityData;
		public override string DisplayName => "AnimationEnumerationChoice";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AnimationEnumerationChoiceEntity(FrostySdk.Ebx.AnimationEnumerationChoiceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

