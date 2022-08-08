using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HudControlOutputEntityData))]
	public class HudControlOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HudControlOutputEntityData>
	{
		public new FrostySdk.Ebx.HudControlOutputEntityData Data => data as FrostySdk.Ebx.HudControlOutputEntityData;
		public override string DisplayName => "HudControlOutput";

		public HudControlOutputEntity(FrostySdk.Ebx.HudControlOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

