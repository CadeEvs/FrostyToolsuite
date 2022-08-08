using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICombatReticleContainerWidgetEntityData))]
	public class UICombatReticleContainerWidgetEntity : UIDynamicSpawnerWidgetEntity, IEntityData<FrostySdk.Ebx.UICombatReticleContainerWidgetEntityData>
	{
		public new FrostySdk.Ebx.UICombatReticleContainerWidgetEntityData Data => data as FrostySdk.Ebx.UICombatReticleContainerWidgetEntityData;
		public override string DisplayName => "UICombatReticleContainerWidget";

		public UICombatReticleContainerWidgetEntity(FrostySdk.Ebx.UICombatReticleContainerWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

