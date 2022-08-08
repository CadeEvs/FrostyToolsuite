using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CustomizedSaveDataProviderData))]
	public class CustomizedSaveDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.CustomizedSaveDataProviderData>
	{
		public new FrostySdk.Ebx.CustomizedSaveDataProviderData Data => data as FrostySdk.Ebx.CustomizedSaveDataProviderData;
		public override string DisplayName => "CustomizedSaveDataProvider";

		public CustomizedSaveDataProvider(FrostySdk.Ebx.CustomizedSaveDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

