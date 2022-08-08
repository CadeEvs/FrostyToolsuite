using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogitechEntityData))]
	public class LogitechEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogitechEntityData>
	{
		public new FrostySdk.Ebx.LogitechEntityData Data => data as FrostySdk.Ebx.LogitechEntityData;
		public override string DisplayName => "Logitech";

		public LogitechEntity(FrostySdk.Ebx.LogitechEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

