using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TimeToStringConverterEntityData))]
	public class TimeToStringConverterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TimeToStringConverterEntityData>
	{
		public new FrostySdk.Ebx.TimeToStringConverterEntityData Data => data as FrostySdk.Ebx.TimeToStringConverterEntityData;
		public override string DisplayName => "TimeToStringConverter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TimeToStringConverterEntity(FrostySdk.Ebx.TimeToStringConverterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

