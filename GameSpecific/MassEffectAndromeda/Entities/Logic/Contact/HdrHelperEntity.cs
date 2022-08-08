using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HdrHelperEntityData))]
	public class HdrHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HdrHelperEntityData>
	{
		public new FrostySdk.Ebx.HdrHelperEntityData Data => data as FrostySdk.Ebx.HdrHelperEntityData;
		public override string DisplayName => "HdrHelper";

		public HdrHelperEntity(FrostySdk.Ebx.HdrHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

