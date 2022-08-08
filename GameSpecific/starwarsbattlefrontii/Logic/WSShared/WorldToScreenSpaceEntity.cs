using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldToScreenSpaceEntityData))]
	public class WorldToScreenSpaceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WorldToScreenSpaceEntityData>
	{
		public new FrostySdk.Ebx.WorldToScreenSpaceEntityData Data => data as FrostySdk.Ebx.WorldToScreenSpaceEntityData;
		public override string DisplayName => "WorldToScreenSpace";

		public WorldToScreenSpaceEntity(FrostySdk.Ebx.WorldToScreenSpaceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

