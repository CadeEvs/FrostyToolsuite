using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureFollowBaseData))]
	public class CreatureFollowBase : LogicEntity, IEntityData<FrostySdk.Ebx.CreatureFollowBaseData>
	{
		public new FrostySdk.Ebx.CreatureFollowBaseData Data => data as FrostySdk.Ebx.CreatureFollowBaseData;
		public override string DisplayName => "CreatureFollowBase";

		public CreatureFollowBase(FrostySdk.Ebx.CreatureFollowBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

