using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIAbilitiesListTraitsEntityData))]
	public class UIAbilitiesListTraitsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIAbilitiesListTraitsEntityData>
	{
		public new FrostySdk.Ebx.UIAbilitiesListTraitsEntityData Data => data as FrostySdk.Ebx.UIAbilitiesListTraitsEntityData;
		public override string DisplayName => "UIAbilitiesListTraits";

		public UIAbilitiesListTraitsEntity(FrostySdk.Ebx.UIAbilitiesListTraitsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

