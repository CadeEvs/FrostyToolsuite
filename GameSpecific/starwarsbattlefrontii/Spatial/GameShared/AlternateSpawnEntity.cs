using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AlternateSpawnEntityData))]
	public class AlternateSpawnEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AlternateSpawnEntityData>
	{
		public new FrostySdk.Ebx.AlternateSpawnEntityData Data => data as FrostySdk.Ebx.AlternateSpawnEntityData;

		public AlternateSpawnEntity(FrostySdk.Ebx.AlternateSpawnEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

