using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProgressionUIDataProviderData))]
	public class ProgressionUIDataProvider : ProgressionUIDataProviderBase, IEntityData<FrostySdk.Ebx.ProgressionUIDataProviderData>
	{
		public new FrostySdk.Ebx.ProgressionUIDataProviderData Data => data as FrostySdk.Ebx.ProgressionUIDataProviderData;
		public override string DisplayName => "ProgressionUIDataProvider";

		public ProgressionUIDataProvider(FrostySdk.Ebx.ProgressionUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

