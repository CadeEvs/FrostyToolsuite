using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocoEntityData))]
	public class LocoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocoEntityData>
	{
		public new FrostySdk.Ebx.LocoEntityData Data => data as FrostySdk.Ebx.LocoEntityData;
		public override string DisplayName => "Loco";

		public LocoEntity(FrostySdk.Ebx.LocoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

