using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeadMorphDataProviderData))]
	public class HeadMorphDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.HeadMorphDataProviderData>
	{
		public new FrostySdk.Ebx.HeadMorphDataProviderData Data => data as FrostySdk.Ebx.HeadMorphDataProviderData;
		public override string DisplayName => "HeadMorphDataProvider";

		public HeadMorphDataProvider(FrostySdk.Ebx.HeadMorphDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

