using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomActionSelectorEntityData))]
	public class RandomActionSelectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomActionSelectorEntityData>
	{
		public new FrostySdk.Ebx.RandomActionSelectorEntityData Data => data as FrostySdk.Ebx.RandomActionSelectorEntityData;
		public override string DisplayName => "RandomActionSelector";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RandomActionSelectorEntity(FrostySdk.Ebx.RandomActionSelectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

