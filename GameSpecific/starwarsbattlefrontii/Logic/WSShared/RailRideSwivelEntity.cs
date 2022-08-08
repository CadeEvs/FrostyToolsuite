using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RailRideSwivelEntityData))]
	public class RailRideSwivelEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RailRideSwivelEntityData>
	{
		public new FrostySdk.Ebx.RailRideSwivelEntityData Data => data as FrostySdk.Ebx.RailRideSwivelEntityData;
		public override string DisplayName => "RailRideSwivel";

		public RailRideSwivelEntity(FrostySdk.Ebx.RailRideSwivelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

