using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StoreUIData))]
	public class StoreUI : LogicEntity, IEntityData<FrostySdk.Ebx.StoreUIData>
	{
		public new FrostySdk.Ebx.StoreUIData Data => data as FrostySdk.Ebx.StoreUIData;
		public override string DisplayName => "StoreUI";

		public StoreUI(FrostySdk.Ebx.StoreUIData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

