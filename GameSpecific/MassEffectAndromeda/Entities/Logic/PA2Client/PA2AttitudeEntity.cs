using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PA2AttitudeEntityData))]
	public class PA2AttitudeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PA2AttitudeEntityData>
	{
		public new FrostySdk.Ebx.PA2AttitudeEntityData Data => data as FrostySdk.Ebx.PA2AttitudeEntityData;
		public override string DisplayName => "PA2Attitude";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Behavior", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enable", Direction.In)
			};
		}

		public PA2AttitudeEntity(FrostySdk.Ebx.PA2AttitudeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

