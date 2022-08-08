using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIAbilityDefaultEntityData))]
	public class UIAbilityDefaultEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIAbilityDefaultEntityData>
	{
		public new FrostySdk.Ebx.UIAbilityDefaultEntityData Data => data as FrostySdk.Ebx.UIAbilityDefaultEntityData;
		public override string DisplayName => "UIAbilityDefault";

		public UIAbilityDefaultEntity(FrostySdk.Ebx.UIAbilityDefaultEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

