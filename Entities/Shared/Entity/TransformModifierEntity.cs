using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformModifierEntityData))]
	public class TransformModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformModifierEntityData>
	{
		public new FrostySdk.Ebx.TransformModifierEntityData Data => data as FrostySdk.Ebx.TransformModifierEntityData;
		public override string DisplayName => "TransformModifier";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TransformModifierEntity(FrostySdk.Ebx.TransformModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

