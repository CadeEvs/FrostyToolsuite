using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeshSpawnerData))]
	public class MeshSpawner : LogicEntity, IEntityData<FrostySdk.Ebx.MeshSpawnerData>
	{
		public new FrostySdk.Ebx.MeshSpawnerData Data => data as FrostySdk.Ebx.MeshSpawnerData;
		public override string DisplayName => "MeshSpawner";

		public MeshSpawner(FrostySdk.Ebx.MeshSpawnerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

