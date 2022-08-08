using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PLMConsolePowerStateEntityData))]
	public class PLMConsolePowerStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PLMConsolePowerStateEntityData>
	{
		public new FrostySdk.Ebx.PLMConsolePowerStateEntityData Data => data as FrostySdk.Ebx.PLMConsolePowerStateEntityData;
		public override string DisplayName => "PLMConsolePowerState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PLMConsolePowerStateEntity(FrostySdk.Ebx.PLMConsolePowerStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

