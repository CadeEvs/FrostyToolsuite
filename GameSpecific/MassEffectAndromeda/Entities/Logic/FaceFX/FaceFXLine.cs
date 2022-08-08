using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FaceFXLineData))]
	public class FaceFXLine : LogicEntity, IEntityData<FrostySdk.Ebx.FaceFXLineData>
	{
		public new FrostySdk.Ebx.FaceFXLineData Data => data as FrostySdk.Ebx.FaceFXLineData;
		public override string DisplayName => "FaceFXLine";

		public FaceFXLine(FrostySdk.Ebx.FaceFXLineData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

