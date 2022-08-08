using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureConfigurationProviderEntityData))]
	public class CreatureConfigurationProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CreatureConfigurationProviderEntityData>
	{
		public new FrostySdk.Ebx.CreatureConfigurationProviderEntityData Data => data as FrostySdk.Ebx.CreatureConfigurationProviderEntityData;
		public override string DisplayName => "CreatureConfigurationProvider";

		public CreatureConfigurationProviderEntity(FrostySdk.Ebx.CreatureConfigurationProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

