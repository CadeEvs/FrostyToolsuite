using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LootEnumerationHelperEntityData))]
	public class LootEnumerationHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LootEnumerationHelperEntityData>
	{
		public new FrostySdk.Ebx.LootEnumerationHelperEntityData Data => data as FrostySdk.Ebx.LootEnumerationHelperEntityData;
		public override string DisplayName => "LootEnumerationHelper";

		public LootEnumerationHelperEntity(FrostySdk.Ebx.LootEnumerationHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

