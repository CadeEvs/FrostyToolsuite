using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadarCombatAreaElementData))]
	public class RadarCombatAreaElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.RadarCombatAreaElementData>
	{
		public new FrostySdk.Ebx.RadarCombatAreaElementData Data => data as FrostySdk.Ebx.RadarCombatAreaElementData;
		public override string DisplayName => "RadarCombatAreaElement";

		public RadarCombatAreaElement(FrostySdk.Ebx.RadarCombatAreaElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

