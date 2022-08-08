using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientDFSimVolumeEntityData))]
	public class ClientDFSimVolumeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientDFSimVolumeEntityData>
	{
		public new FrostySdk.Ebx.ClientDFSimVolumeEntityData Data => data as FrostySdk.Ebx.ClientDFSimVolumeEntityData;
		public override string DisplayName => "ClientDFSimVolume";

		public ClientDFSimVolumeEntity(FrostySdk.Ebx.ClientDFSimVolumeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

