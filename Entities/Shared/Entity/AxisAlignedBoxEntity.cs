using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AxisAlignedBoxEntityData))]
	public class AxisAlignedBoxEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AxisAlignedBoxEntityData>
	{
		public new FrostySdk.Ebx.AxisAlignedBoxEntityData Data => data as FrostySdk.Ebx.AxisAlignedBoxEntityData;
		public override string DisplayName => "AxisAlignedBox";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AxisAlignedBoxEntity(FrostySdk.Ebx.AxisAlignedBoxEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

