using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VehicleSummonerEntityData))]
	public class VehicleSummonerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VehicleSummonerEntityData>
	{
		public new FrostySdk.Ebx.VehicleSummonerEntityData Data => data as FrostySdk.Ebx.VehicleSummonerEntityData;
		public override string DisplayName => "VehicleSummoner";

		public VehicleSummonerEntity(FrostySdk.Ebx.VehicleSummonerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

