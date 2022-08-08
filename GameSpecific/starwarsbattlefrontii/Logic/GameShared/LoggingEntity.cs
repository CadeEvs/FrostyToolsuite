using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoggingEntityData))]
	public class LoggingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LoggingEntityData>
	{
		public new FrostySdk.Ebx.LoggingEntityData Data => data as FrostySdk.Ebx.LoggingEntityData;
		public override string DisplayName => "Logging";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LoggingEntity(FrostySdk.Ebx.LoggingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

