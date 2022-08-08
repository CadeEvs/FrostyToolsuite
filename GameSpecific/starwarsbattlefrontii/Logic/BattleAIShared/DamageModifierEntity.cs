using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DamageModifierEntityData))]
	public class DamageModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DamageModifierEntityData>
	{
		public new FrostySdk.Ebx.DamageModifierEntityData Data => data as FrostySdk.Ebx.DamageModifierEntityData;
		public override string DisplayName => "DamageModifier";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DamageModifierEntity(FrostySdk.Ebx.DamageModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

