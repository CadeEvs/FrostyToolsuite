using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MPProgressionUIDataProviderData))]
	public class MPProgressionUIDataProvider : ProgressionUIDataProviderBase, IEntityData<FrostySdk.Ebx.MPProgressionUIDataProviderData>
	{
		public new FrostySdk.Ebx.MPProgressionUIDataProviderData Data => data as FrostySdk.Ebx.MPProgressionUIDataProviderData;
		public override string DisplayName => "MPProgressionUIDataProvider";

		public MPProgressionUIDataProvider(FrostySdk.Ebx.MPProgressionUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

