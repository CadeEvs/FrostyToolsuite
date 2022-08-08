using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HudControlInputEntityData))]
	public class HudControlInputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HudControlInputEntityData>
	{
		public new FrostySdk.Ebx.HudControlInputEntityData Data => data as FrostySdk.Ebx.HudControlInputEntityData;
		public override string DisplayName => "HudControlInput";

		public HudControlInputEntity(FrostySdk.Ebx.HudControlInputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

