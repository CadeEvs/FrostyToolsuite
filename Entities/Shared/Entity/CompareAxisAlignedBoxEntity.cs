using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareAxisAlignedBoxEntityData))]
	public class CompareAxisAlignedBoxEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareAxisAlignedBoxEntityData>
	{
		public new FrostySdk.Ebx.CompareAxisAlignedBoxEntityData Data => data as FrostySdk.Ebx.CompareAxisAlignedBoxEntityData;
		public override string DisplayName => "CompareAxisAlignedBox";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CompareAxisAlignedBoxEntity(FrostySdk.Ebx.CompareAxisAlignedBoxEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

