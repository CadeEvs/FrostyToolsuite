using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ServerTargetComputerValidationEntityData))]
	public class ServerTargetComputerValidationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ServerTargetComputerValidationEntityData>
	{
		public new FrostySdk.Ebx.ServerTargetComputerValidationEntityData Data => data as FrostySdk.Ebx.ServerTargetComputerValidationEntityData;
		public override string DisplayName => "ServerTargetComputerValidation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ServerTargetComputerValidationEntity(FrostySdk.Ebx.ServerTargetComputerValidationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

