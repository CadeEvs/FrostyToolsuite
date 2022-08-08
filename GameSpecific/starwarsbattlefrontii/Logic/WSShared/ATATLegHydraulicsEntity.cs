using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ATATLegHydraulicsEntityData))]
	public class ATATLegHydraulicsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ATATLegHydraulicsEntityData>
	{
		public new FrostySdk.Ebx.ATATLegHydraulicsEntityData Data => data as FrostySdk.Ebx.ATATLegHydraulicsEntityData;
		public override string DisplayName => "ATATLegHydraulics";

		public ATATLegHydraulicsEntity(FrostySdk.Ebx.ATATLegHydraulicsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

