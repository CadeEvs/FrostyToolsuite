using System.Collections.Generic;
using System.IO;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEEquipItemEntityData))]
	public class MEEquipItemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEEquipItemEntityData>
	{
		public new FrostySdk.Ebx.MEEquipItemEntityData Data => data as FrostySdk.Ebx.MEEquipItemEntityData;
		public override string DisplayName => "MEEquipItem";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("EquipItem", Direction.In)
			};
		}
        public override IEnumerable<string> HeaderRows
        {
			get
            {
				List<string> outHeaderRows = new List<string>();
				string itemName = Assets.ItemData.GetItemNameFromHash(Data.ItemHash);
				if (itemName != "")
				{
					outHeaderRows.Add($"Item: {Path.GetFileName(itemName)}");
				}
				return outHeaderRows;
            }
        }

        public MEEquipItemEntity(FrostySdk.Ebx.MEEquipItemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

