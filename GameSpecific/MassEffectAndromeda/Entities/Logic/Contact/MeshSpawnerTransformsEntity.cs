using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MeshSpawnerTransformsEntityData))]
	public class MeshSpawnerTransformsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MeshSpawnerTransformsEntityData>
	{
		public new FrostySdk.Ebx.MeshSpawnerTransformsEntityData Data => data as FrostySdk.Ebx.MeshSpawnerTransformsEntityData;
		public override string DisplayName => "MeshSpawnerTransforms";

		public MeshSpawnerTransformsEntity(FrostySdk.Ebx.MeshSpawnerTransformsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

