using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CheckpointDistanceEntityData))]
	public class CheckpointDistanceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CheckpointDistanceEntityData>
	{
		public new FrostySdk.Ebx.CheckpointDistanceEntityData Data => data as FrostySdk.Ebx.CheckpointDistanceEntityData;
		public override string DisplayName => "CheckpointDistance";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CheckpointDistanceEntity(FrostySdk.Ebx.CheckpointDistanceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

