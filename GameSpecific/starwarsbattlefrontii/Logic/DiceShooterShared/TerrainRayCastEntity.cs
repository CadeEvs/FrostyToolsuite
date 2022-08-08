using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainRayCastEntityData))]
	public class TerrainRayCastEntity : RayCastEntity, IEntityData<FrostySdk.Ebx.TerrainRayCastEntityData>
	{
		public new FrostySdk.Ebx.TerrainRayCastEntityData Data => data as FrostySdk.Ebx.TerrainRayCastEntityData;
		public override string DisplayName => "TerrainRayCast";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TerrainRayCastEntity(FrostySdk.Ebx.TerrainRayCastEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

