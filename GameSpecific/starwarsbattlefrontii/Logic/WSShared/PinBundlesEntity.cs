using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PinBundlesEntityData))]
	public class PinBundlesEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PinBundlesEntityData>
	{
		public new FrostySdk.Ebx.PinBundlesEntityData Data => data as FrostySdk.Ebx.PinBundlesEntityData;
		public override string DisplayName => "PinBundles";

		public PinBundlesEntity(FrostySdk.Ebx.PinBundlesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

