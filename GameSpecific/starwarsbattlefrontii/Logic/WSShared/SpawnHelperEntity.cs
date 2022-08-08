using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnHelperEntityData))]
	public class SpawnHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnHelperEntityData>
	{
		public new FrostySdk.Ebx.SpawnHelperEntityData Data => data as FrostySdk.Ebx.SpawnHelperEntityData;
		public override string DisplayName => "SpawnHelper";

		public SpawnHelperEntity(FrostySdk.Ebx.SpawnHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

