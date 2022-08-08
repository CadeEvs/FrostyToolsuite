using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SquadFilterEntityData))]
	public class SquadFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SquadFilterEntityData>
	{
		public new FrostySdk.Ebx.SquadFilterEntityData Data => data as FrostySdk.Ebx.SquadFilterEntityData;
		public override string DisplayName => "SquadFilter";

		public SquadFilterEntity(FrostySdk.Ebx.SquadFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

