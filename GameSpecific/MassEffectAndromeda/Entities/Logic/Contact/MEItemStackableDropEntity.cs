using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEItemStackableDropEntityData))]
	public class MEItemStackableDropEntity : MEItemActionEntity, IEntityData<FrostySdk.Ebx.MEItemStackableDropEntityData>
	{
		public new FrostySdk.Ebx.MEItemStackableDropEntityData Data => data as FrostySdk.Ebx.MEItemStackableDropEntityData;
		public override string DisplayName => "MEItemStackableDrop";

		public MEItemStackableDropEntity(FrostySdk.Ebx.MEItemStackableDropEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

