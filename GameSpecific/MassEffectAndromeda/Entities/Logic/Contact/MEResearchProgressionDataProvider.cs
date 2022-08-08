using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEResearchProgressionDataProviderData))]
	public class MEResearchProgressionDataProvider : MeshSpawner, IEntityData<FrostySdk.Ebx.MEResearchProgressionDataProviderData>
	{
		public new FrostySdk.Ebx.MEResearchProgressionDataProviderData Data => data as FrostySdk.Ebx.MEResearchProgressionDataProviderData;
		public override string DisplayName => "MEResearchProgressionDataProvider";

		public MEResearchProgressionDataProvider(FrostySdk.Ebx.MEResearchProgressionDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

