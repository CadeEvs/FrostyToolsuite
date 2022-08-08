using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InventoryWheelDataProviderData))]
	public class InventoryWheelDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.InventoryWheelDataProviderData>
	{
		public new FrostySdk.Ebx.InventoryWheelDataProviderData Data => data as FrostySdk.Ebx.InventoryWheelDataProviderData;
		public override string DisplayName => "InventoryWheelDataProvider";

		public InventoryWheelDataProvider(FrostySdk.Ebx.InventoryWheelDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

