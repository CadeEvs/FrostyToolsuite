using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AbilityResourceEntityData))]
	public class AbilityResourceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AbilityResourceEntityData>
	{
		public new FrostySdk.Ebx.AbilityResourceEntityData Data => data as FrostySdk.Ebx.AbilityResourceEntityData;
		public override string DisplayName => "AbilityResource";

		public AbilityResourceEntity(FrostySdk.Ebx.AbilityResourceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

