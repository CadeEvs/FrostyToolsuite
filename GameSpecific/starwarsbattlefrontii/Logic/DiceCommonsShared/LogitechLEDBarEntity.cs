using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogitechLEDBarEntityData))]
	public class LogitechLEDBarEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogitechLEDBarEntityData>
	{
		public new FrostySdk.Ebx.LogitechLEDBarEntityData Data => data as FrostySdk.Ebx.LogitechLEDBarEntityData;
		public override string DisplayName => "LogitechLEDBar";

		public LogitechLEDBarEntity(FrostySdk.Ebx.LogitechLEDBarEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

