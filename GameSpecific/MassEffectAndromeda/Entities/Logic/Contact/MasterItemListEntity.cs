using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MasterItemListEntityData))]
	public class MasterItemListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MasterItemListEntityData>
	{
		public new FrostySdk.Ebx.MasterItemListEntityData Data => data as FrostySdk.Ebx.MasterItemListEntityData;
		public override string DisplayName => "MasterItemList";

		public MasterItemListEntity(FrostySdk.Ebx.MasterItemListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

