using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ContainsAbilityIdDataEntityData))]
	public class ContainsAbilityIdDataEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.ContainsAbilityIdDataEntityData>
	{
		public new FrostySdk.Ebx.ContainsAbilityIdDataEntityData Data => data as FrostySdk.Ebx.ContainsAbilityIdDataEntityData;
		public override string DisplayName => "ContainsAbilityIdData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ContainsAbilityIdDataEntity(FrostySdk.Ebx.ContainsAbilityIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

