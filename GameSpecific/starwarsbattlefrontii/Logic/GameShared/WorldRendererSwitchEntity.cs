using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldRendererSwitchEntityData))]
	public class WorldRendererSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WorldRendererSwitchEntityData>
	{
		public new FrostySdk.Ebx.WorldRendererSwitchEntityData Data => data as FrostySdk.Ebx.WorldRendererSwitchEntityData;
		public override string DisplayName => "WorldRendererSwitch";

		public WorldRendererSwitchEntity(FrostySdk.Ebx.WorldRendererSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

