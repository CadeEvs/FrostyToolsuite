using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSHealthModifierEntityData))]
	public class WSHealthModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSHealthModifierEntityData>
	{
		public new FrostySdk.Ebx.WSHealthModifierEntityData Data => data as FrostySdk.Ebx.WSHealthModifierEntityData;
		public override string DisplayName => "WSHealthModifier";

		public WSHealthModifierEntity(FrostySdk.Ebx.WSHealthModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

