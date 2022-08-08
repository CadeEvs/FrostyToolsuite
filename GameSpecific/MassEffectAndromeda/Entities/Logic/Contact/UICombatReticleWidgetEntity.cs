using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICombatReticleWidgetEntityData))]
	public class UICombatReticleWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UICombatReticleWidgetEntityData>
	{
		public new FrostySdk.Ebx.UICombatReticleWidgetEntityData Data => data as FrostySdk.Ebx.UICombatReticleWidgetEntityData;
		public override string DisplayName => "UICombatReticleWidget";

		public UICombatReticleWidgetEntity(FrostySdk.Ebx.UICombatReticleWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

