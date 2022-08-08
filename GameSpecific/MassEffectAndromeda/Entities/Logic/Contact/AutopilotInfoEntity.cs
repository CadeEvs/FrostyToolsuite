using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutopilotInfoEntityData))]
	public class AutopilotInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AutopilotInfoEntityData>
	{
		public new FrostySdk.Ebx.AutopilotInfoEntityData Data => data as FrostySdk.Ebx.AutopilotInfoEntityData;
		public override string DisplayName => "AutopilotInfo";

		public AutopilotInfoEntity(FrostySdk.Ebx.AutopilotInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

