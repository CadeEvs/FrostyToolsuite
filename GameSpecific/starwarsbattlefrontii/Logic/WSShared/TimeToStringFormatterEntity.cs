using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TimeToStringFormatterEntityData))]
	public class TimeToStringFormatterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TimeToStringFormatterEntityData>
	{
		public new FrostySdk.Ebx.TimeToStringFormatterEntityData Data => data as FrostySdk.Ebx.TimeToStringFormatterEntityData;
		public override string DisplayName => "TimeToStringFormatter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TimeToStringFormatterEntity(FrostySdk.Ebx.TimeToStringFormatterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

