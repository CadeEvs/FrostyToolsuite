using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetTeamDirectiveEntityData))]
	public class SetTeamDirectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetTeamDirectiveEntityData>
	{
		public new FrostySdk.Ebx.SetTeamDirectiveEntityData Data => data as FrostySdk.Ebx.SetTeamDirectiveEntityData;
		public override string DisplayName => "SetTeamDirective";

		public SetTeamDirectiveEntity(FrostySdk.Ebx.SetTeamDirectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

