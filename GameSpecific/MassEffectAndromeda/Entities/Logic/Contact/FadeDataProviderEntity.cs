using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FadeDataProviderEntityData))]
	public class FadeDataProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FadeDataProviderEntityData>
	{
		public new FrostySdk.Ebx.FadeDataProviderEntityData Data => data as FrostySdk.Ebx.FadeDataProviderEntityData;
		public override string DisplayName => "FadeDataProvider";

		public FadeDataProviderEntity(FrostySdk.Ebx.FadeDataProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

