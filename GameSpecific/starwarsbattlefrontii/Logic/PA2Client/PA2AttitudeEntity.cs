using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PA2AttitudeEntityData))]
	public class PA2AttitudeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PA2AttitudeEntityData>
	{
		public new FrostySdk.Ebx.PA2AttitudeEntityData Data => data as FrostySdk.Ebx.PA2AttitudeEntityData;
		public override string DisplayName => "PA2Attitude";

		public PA2AttitudeEntity(FrostySdk.Ebx.PA2AttitudeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

