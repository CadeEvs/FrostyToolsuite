using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MissionDataProviderData))]
	public class MissionDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.MissionDataProviderData>
	{
		public new FrostySdk.Ebx.MissionDataProviderData Data => data as FrostySdk.Ebx.MissionDataProviderData;
		public override string DisplayName => "MissionDataProvider";

		public MissionDataProvider(FrostySdk.Ebx.MissionDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

