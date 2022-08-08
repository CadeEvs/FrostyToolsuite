using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlurredRegionSourceEntityData))]
	public class BlurredRegionSourceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlurredRegionSourceEntityData>
	{
		public new FrostySdk.Ebx.BlurredRegionSourceEntityData Data => data as FrostySdk.Ebx.BlurredRegionSourceEntityData;
		public override string DisplayName => "BlurredRegionSource";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlurredRegionSourceEntity(FrostySdk.Ebx.BlurredRegionSourceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

