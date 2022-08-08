using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicSpawnLinkEntityData))]
	public class DynamicSpawnLinkEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicSpawnLinkEntityData>
	{
		public new FrostySdk.Ebx.DynamicSpawnLinkEntityData Data => data as FrostySdk.Ebx.DynamicSpawnLinkEntityData;
		public override string DisplayName => "DynamicSpawnLink";

		public DynamicSpawnLinkEntity(FrostySdk.Ebx.DynamicSpawnLinkEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

