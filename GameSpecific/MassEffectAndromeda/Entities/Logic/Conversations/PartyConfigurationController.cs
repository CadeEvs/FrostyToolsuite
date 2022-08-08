using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyConfigurationControllerData))]
	public class PartyConfigurationController : LogicEntity, IEntityData<FrostySdk.Ebx.PartyConfigurationControllerData>
	{
		public new FrostySdk.Ebx.PartyConfigurationControllerData Data => data as FrostySdk.Ebx.PartyConfigurationControllerData;
		public override string DisplayName => "PartyConfigurationController";

		public PartyConfigurationController(FrostySdk.Ebx.PartyConfigurationControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

