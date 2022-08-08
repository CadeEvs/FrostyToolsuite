using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayAnimationEntityData))]
	public class PlayAnimationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayAnimationEntityData>
	{
		public new FrostySdk.Ebx.PlayAnimationEntityData Data => data as FrostySdk.Ebx.PlayAnimationEntityData;
		public override string DisplayName => "PlayAnimation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayAnimationEntity(FrostySdk.Ebx.PlayAnimationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

