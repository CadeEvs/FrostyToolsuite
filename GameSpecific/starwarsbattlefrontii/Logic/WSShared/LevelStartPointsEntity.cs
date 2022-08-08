using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LevelStartPointsEntityData))]
	public class LevelStartPointsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LevelStartPointsEntityData>
	{
		public new FrostySdk.Ebx.LevelStartPointsEntityData Data => data as FrostySdk.Ebx.LevelStartPointsEntityData;
		public override string DisplayName => "LevelStartPoints";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LevelStartPointsEntity(FrostySdk.Ebx.LevelStartPointsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

