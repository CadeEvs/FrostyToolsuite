using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WallOfDoomEntityData))]
	public class WallOfDoomEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WallOfDoomEntityData>
	{
		public new FrostySdk.Ebx.WallOfDoomEntityData Data => data as FrostySdk.Ebx.WallOfDoomEntityData;
		public override string DisplayName => "WallOfDoom";

		public WallOfDoomEntity(FrostySdk.Ebx.WallOfDoomEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

