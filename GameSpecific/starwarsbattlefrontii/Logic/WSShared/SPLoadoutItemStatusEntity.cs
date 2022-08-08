using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPLoadoutItemStatusEntityData))]
	public class SPLoadoutItemStatusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPLoadoutItemStatusEntityData>
	{
		public new FrostySdk.Ebx.SPLoadoutItemStatusEntityData Data => data as FrostySdk.Ebx.SPLoadoutItemStatusEntityData;
		public override string DisplayName => "SPLoadoutItemStatus";

		public SPLoadoutItemStatusEntity(FrostySdk.Ebx.SPLoadoutItemStatusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

