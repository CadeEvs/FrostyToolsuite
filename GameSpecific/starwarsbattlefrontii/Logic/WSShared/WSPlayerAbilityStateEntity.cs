using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSPlayerAbilityStateEntityData))]
	public class WSPlayerAbilityStateEntity : PlayerAbilityStateEntity, IEntityData<FrostySdk.Ebx.WSPlayerAbilityStateEntityData>
	{
		public new FrostySdk.Ebx.WSPlayerAbilityStateEntityData Data => data as FrostySdk.Ebx.WSPlayerAbilityStateEntityData;
		public override string DisplayName => "WSPlayerAbilityState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSPlayerAbilityStateEntity(FrostySdk.Ebx.WSPlayerAbilityStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

