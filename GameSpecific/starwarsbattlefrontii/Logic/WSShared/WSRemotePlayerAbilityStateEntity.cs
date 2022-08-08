using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSRemotePlayerAbilityStateEntityData))]
	public class WSRemotePlayerAbilityStateEntity : RemotePlayerAbilityStateEntity, IEntityData<FrostySdk.Ebx.WSRemotePlayerAbilityStateEntityData>
	{
		public new FrostySdk.Ebx.WSRemotePlayerAbilityStateEntityData Data => data as FrostySdk.Ebx.WSRemotePlayerAbilityStateEntityData;
		public override string DisplayName => "WSRemotePlayerAbilityState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSRemotePlayerAbilityStateEntity(FrostySdk.Ebx.WSRemotePlayerAbilityStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

