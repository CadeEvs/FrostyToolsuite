using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnLocationFinderEntityData))]
	public class SpawnLocationFinderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnLocationFinderEntityData>
	{
		public new FrostySdk.Ebx.SpawnLocationFinderEntityData Data => data as FrostySdk.Ebx.SpawnLocationFinderEntityData;
		public override string DisplayName => "SpawnLocationFinder";

		public SpawnLocationFinderEntity(FrostySdk.Ebx.SpawnLocationFinderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

