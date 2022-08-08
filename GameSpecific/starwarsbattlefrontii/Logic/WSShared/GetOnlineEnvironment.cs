using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetOnlineEnvironmentData))]
	public class GetOnlineEnvironment : LogicEntity, IEntityData<FrostySdk.Ebx.GetOnlineEnvironmentData>
	{
		public new FrostySdk.Ebx.GetOnlineEnvironmentData Data => data as FrostySdk.Ebx.GetOnlineEnvironmentData;
		public override string DisplayName => "GetOnlineEnvironment";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GetOnlineEnvironment(FrostySdk.Ebx.GetOnlineEnvironmentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

