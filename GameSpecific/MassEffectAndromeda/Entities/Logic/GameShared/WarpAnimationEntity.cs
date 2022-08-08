using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WarpAnimationEntityData))]
	public class WarpAnimationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WarpAnimationEntityData>
	{
		public new FrostySdk.Ebx.WarpAnimationEntityData Data => data as FrostySdk.Ebx.WarpAnimationEntityData;
		public override string DisplayName => "WarpAnimation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WarpAnimationEntity(FrostySdk.Ebx.WarpAnimationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

