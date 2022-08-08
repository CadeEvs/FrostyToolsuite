using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HidePlayerFilterDummyEntityData))]
	public class HidePlayerFilterDummyEntity : PlayerFilterEntity, IEntityData<FrostySdk.Ebx.HidePlayerFilterDummyEntityData>
	{
		public new FrostySdk.Ebx.HidePlayerFilterDummyEntityData Data => data as FrostySdk.Ebx.HidePlayerFilterDummyEntityData;
		public override string DisplayName => "HidePlayerFilterDummy";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public HidePlayerFilterDummyEntity(FrostySdk.Ebx.HidePlayerFilterDummyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

