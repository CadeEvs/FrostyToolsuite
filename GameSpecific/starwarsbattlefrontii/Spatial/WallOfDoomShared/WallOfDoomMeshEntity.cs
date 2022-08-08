using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WallOfDoomMeshEntityData))]
	public class WallOfDoomMeshEntity : StaticModelEntity, IEntityData<FrostySdk.Ebx.WallOfDoomMeshEntityData>
	{
		public new FrostySdk.Ebx.WallOfDoomMeshEntityData Data => data as FrostySdk.Ebx.WallOfDoomMeshEntityData;

		public WallOfDoomMeshEntity(FrostySdk.Ebx.WallOfDoomMeshEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

