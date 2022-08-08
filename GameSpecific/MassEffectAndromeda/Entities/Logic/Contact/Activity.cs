using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ActivityData))]
	public class Activity : LogicEntity, IEntityData<FrostySdk.Ebx.ActivityData>
	{
		public new FrostySdk.Ebx.ActivityData Data => data as FrostySdk.Ebx.ActivityData;
		public override string DisplayName => "Activity";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Activity(FrostySdk.Ebx.ActivityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

