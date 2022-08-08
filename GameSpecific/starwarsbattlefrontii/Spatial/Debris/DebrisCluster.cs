using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebrisClusterData))]
	public class DebrisCluster : GameComponentEntity, IEntityData<FrostySdk.Ebx.DebrisClusterData>
	{
		public new FrostySdk.Ebx.DebrisClusterData Data => data as FrostySdk.Ebx.DebrisClusterData;

		public DebrisCluster(FrostySdk.Ebx.DebrisClusterData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

