using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreateMatchData))]
	public class CreateMatch : LogicEntity, IEntityData<FrostySdk.Ebx.CreateMatchData>
	{
		public new FrostySdk.Ebx.CreateMatchData Data => data as FrostySdk.Ebx.CreateMatchData;
		public override string DisplayName => "CreateMatch";

		public CreateMatch(FrostySdk.Ebx.CreateMatchData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

