using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESaveGameDataProviderData))]
	public class MESaveGameDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.MESaveGameDataProviderData>
	{
		public new FrostySdk.Ebx.MESaveGameDataProviderData Data => data as FrostySdk.Ebx.MESaveGameDataProviderData;
		public override string DisplayName => "MESaveGameDataProvider";

		public MESaveGameDataProvider(FrostySdk.Ebx.MESaveGameDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

