using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnGroupManagerEntityData))]
	public class SpawnGroupManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpawnGroupManagerEntityData>
	{
		public new FrostySdk.Ebx.SpawnGroupManagerEntityData Data => data as FrostySdk.Ebx.SpawnGroupManagerEntityData;
		public override string DisplayName => "SpawnGroupManager";

		public SpawnGroupManagerEntity(FrostySdk.Ebx.SpawnGroupManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

