using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPLoadoutViewItemEntityData))]
	public class SPLoadoutViewItemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPLoadoutViewItemEntityData>
	{
		public new FrostySdk.Ebx.SPLoadoutViewItemEntityData Data => data as FrostySdk.Ebx.SPLoadoutViewItemEntityData;
		public override string DisplayName => "SPLoadoutViewItem";

		public SPLoadoutViewItemEntity(FrostySdk.Ebx.SPLoadoutViewItemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

