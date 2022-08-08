using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEItemDropEntityData))]
	public class MEItemDropEntity : MEItemActionEntity, IEntityData<FrostySdk.Ebx.MEItemDropEntityData>
	{
		public new FrostySdk.Ebx.MEItemDropEntityData Data => data as FrostySdk.Ebx.MEItemDropEntityData;
		public override string DisplayName => "MEItemDrop";

		public MEItemDropEntity(FrostySdk.Ebx.MEItemDropEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

