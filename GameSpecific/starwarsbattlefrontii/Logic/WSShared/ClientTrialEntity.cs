using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientTrialEntityData))]
	public class ClientTrialEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientTrialEntityData>
	{
		public new FrostySdk.Ebx.ClientTrialEntityData Data => data as FrostySdk.Ebx.ClientTrialEntityData;
		public override string DisplayName => "ClientTrial";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ClientTrialEntity(FrostySdk.Ebx.ClientTrialEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

