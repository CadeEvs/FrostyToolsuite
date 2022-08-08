using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SlowMotionEntityData))]
	public class SlowMotionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SlowMotionEntityData>
	{
		public new FrostySdk.Ebx.SlowMotionEntityData Data => data as FrostySdk.Ebx.SlowMotionEntityData;
		public override string DisplayName => "SlowMotion";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SlowMotionEntity(FrostySdk.Ebx.SlowMotionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

