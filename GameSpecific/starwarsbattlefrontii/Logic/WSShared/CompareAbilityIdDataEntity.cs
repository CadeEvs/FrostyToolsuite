using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareAbilityIdDataEntityData))]
	public class CompareAbilityIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareAbilityIdDataEntityData>
	{
		public new FrostySdk.Ebx.CompareAbilityIdDataEntityData Data => data as FrostySdk.Ebx.CompareAbilityIdDataEntityData;
		public override string DisplayName => "CompareAbilityIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareAbilityIdDataEntity(FrostySdk.Ebx.CompareAbilityIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

