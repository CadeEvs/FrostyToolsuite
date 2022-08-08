using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSHealthComponentDamageModifierEntityData))]
	public class WSHealthComponentDamageModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSHealthComponentDamageModifierEntityData>
	{
		public new FrostySdk.Ebx.WSHealthComponentDamageModifierEntityData Data => data as FrostySdk.Ebx.WSHealthComponentDamageModifierEntityData;
		public override string DisplayName => "WSHealthComponentDamageModifier";

		public WSHealthComponentDamageModifierEntity(FrostySdk.Ebx.WSHealthComponentDamageModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

