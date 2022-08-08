using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetTeamFilterEntityData))]
	public class TargetTeamFilterEntity : TargetFilterEntity, IEntityData<FrostySdk.Ebx.TargetTeamFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetTeamFilterEntityData Data => data as FrostySdk.Ebx.TargetTeamFilterEntityData;
		public override string DisplayName => "TargetTeamFilter";

		public TargetTeamFilterEntity(FrostySdk.Ebx.TargetTeamFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

