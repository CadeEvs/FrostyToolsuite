using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LightDimmerEntityData))]
	public class LightDimmerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LightDimmerEntityData>
	{
		public new FrostySdk.Ebx.LightDimmerEntityData Data => data as FrostySdk.Ebx.LightDimmerEntityData;
		public override string DisplayName => "LightDimmer";

		public LightDimmerEntity(FrostySdk.Ebx.LightDimmerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

