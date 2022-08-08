using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldMapAreaEntityData))]
	public class WorldMapAreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WorldMapAreaEntityData>
	{
		public new FrostySdk.Ebx.WorldMapAreaEntityData Data => data as FrostySdk.Ebx.WorldMapAreaEntityData;
		public override string DisplayName => "WorldMapArea";

		public WorldMapAreaEntity(FrostySdk.Ebx.WorldMapAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

