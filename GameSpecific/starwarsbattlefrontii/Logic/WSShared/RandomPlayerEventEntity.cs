using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomPlayerEventEntityData))]
	public class RandomPlayerEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomPlayerEventEntityData>
	{
		public new FrostySdk.Ebx.RandomPlayerEventEntityData Data => data as FrostySdk.Ebx.RandomPlayerEventEntityData;
		public override string DisplayName => "RandomPlayerEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RandomPlayerEventEntity(FrostySdk.Ebx.RandomPlayerEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

