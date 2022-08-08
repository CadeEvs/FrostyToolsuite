using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICurrentAbilitiesEntityData))]
	public class UICurrentAbilitiesEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICurrentAbilitiesEntityData>
	{
		public new FrostySdk.Ebx.UICurrentAbilitiesEntityData Data => data as FrostySdk.Ebx.UICurrentAbilitiesEntityData;
		public override string DisplayName => "UICurrentAbilities";

		public UICurrentAbilitiesEntity(FrostySdk.Ebx.UICurrentAbilitiesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

