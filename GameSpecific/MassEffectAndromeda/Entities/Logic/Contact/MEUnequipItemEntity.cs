using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEUnequipItemEntityData))]
	public class MEUnequipItemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEUnequipItemEntityData>
	{
		public new FrostySdk.Ebx.MEUnequipItemEntityData Data => data as FrostySdk.Ebx.MEUnequipItemEntityData;
		public override string DisplayName => "MEUnequipItem";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("TargetEntity", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("UnequipItem", Direction.In)
			};
		}

		public MEUnequipItemEntity(FrostySdk.Ebx.MEUnequipItemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

