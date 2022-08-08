using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HudPlanesEntityData))]
	public class HudPlanesEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HudPlanesEntityData>
	{
		public new FrostySdk.Ebx.HudPlanesEntityData Data => data as FrostySdk.Ebx.HudPlanesEntityData;
		public override string DisplayName => "HudPlanes";

		public HudPlanesEntity(FrostySdk.Ebx.HudPlanesEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

