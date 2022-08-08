using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientAudioVehicleEngineParametersEntityData))]
	public class ClientAudioVehicleEngineParametersEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientAudioVehicleEngineParametersEntityData>
	{
		public new FrostySdk.Ebx.ClientAudioVehicleEngineParametersEntityData Data => data as FrostySdk.Ebx.ClientAudioVehicleEngineParametersEntityData;
		public override string DisplayName => "ClientAudioVehicleEngineParameters";

		public ClientAudioVehicleEngineParametersEntity(FrostySdk.Ebx.ClientAudioVehicleEngineParametersEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

