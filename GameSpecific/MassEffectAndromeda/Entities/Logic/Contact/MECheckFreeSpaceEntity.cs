using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECheckFreeSpaceEntityData))]
	public class MECheckFreeSpaceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MECheckFreeSpaceEntityData>
	{
		public new FrostySdk.Ebx.MECheckFreeSpaceEntityData Data => data as FrostySdk.Ebx.MECheckFreeSpaceEntityData;
		public override string DisplayName => "MECheckFreeSpace";

		public MECheckFreeSpaceEntity(FrostySdk.Ebx.MECheckFreeSpaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

