using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GlobalWeatheringParamsEntityData))]
	public class GlobalWeatheringParamsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GlobalWeatheringParamsEntityData>
	{
		public new FrostySdk.Ebx.GlobalWeatheringParamsEntityData Data => data as FrostySdk.Ebx.GlobalWeatheringParamsEntityData;
		public override string DisplayName => "GlobalWeatheringParams";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public GlobalWeatheringParamsEntity(FrostySdk.Ebx.GlobalWeatheringParamsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

