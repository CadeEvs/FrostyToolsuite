using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemotePlayerAbilityStateEntityData))]
	public class RemotePlayerAbilityStateEntity : PlayerAbilityStateEntity, IEntityData<FrostySdk.Ebx.RemotePlayerAbilityStateEntityData>
	{
		public new FrostySdk.Ebx.RemotePlayerAbilityStateEntityData Data => data as FrostySdk.Ebx.RemotePlayerAbilityStateEntityData;
		public override string DisplayName => "RemotePlayerAbilityState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RemotePlayerAbilityStateEntity(FrostySdk.Ebx.RemotePlayerAbilityStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

