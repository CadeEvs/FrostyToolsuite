using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DifficultyIndexEntityData))]
	public class DifficultyIndexEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DifficultyIndexEntityData>
	{
		public new FrostySdk.Ebx.DifficultyIndexEntityData Data => data as FrostySdk.Ebx.DifficultyIndexEntityData;
		public override string DisplayName => "DifficultyIndex";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DifficultyIndexEntity(FrostySdk.Ebx.DifficultyIndexEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

