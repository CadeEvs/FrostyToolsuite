using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureLocoEntityData))]
	public class CreatureLocoEntity : LocoEntity, IEntityData<FrostySdk.Ebx.CreatureLocoEntityData>
	{
		public new FrostySdk.Ebx.CreatureLocoEntityData Data => data as FrostySdk.Ebx.CreatureLocoEntityData;
		public override string DisplayName => "CreatureLoco";

		public CreatureLocoEntity(FrostySdk.Ebx.CreatureLocoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

