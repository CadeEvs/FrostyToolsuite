using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSCreatureConfigurationProviderEntityData))]
	public class WSCreatureConfigurationProviderEntity : CreatureConfigurationProviderEntity, IEntityData<FrostySdk.Ebx.WSCreatureConfigurationProviderEntityData>
	{
		public new FrostySdk.Ebx.WSCreatureConfigurationProviderEntityData Data => data as FrostySdk.Ebx.WSCreatureConfigurationProviderEntityData;
		public override string DisplayName => "WSCreatureConfigurationProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSCreatureConfigurationProviderEntity(FrostySdk.Ebx.WSCreatureConfigurationProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

