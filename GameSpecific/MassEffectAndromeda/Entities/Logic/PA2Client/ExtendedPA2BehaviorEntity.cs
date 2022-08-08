using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExtendedPA2BehaviorEntityData))]
	public class ExtendedPA2BehaviorEntity : PA2BehaviorEntity, IEntityData<FrostySdk.Ebx.ExtendedPA2BehaviorEntityData>
	{
		public new FrostySdk.Ebx.ExtendedPA2BehaviorEntityData Data => data as FrostySdk.Ebx.ExtendedPA2BehaviorEntityData;
		public override string DisplayName => "ExtendedPA2Behavior";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Animatable", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OverridePrimaryTargetPosition", Direction.In),
				new ConnectionDesc("OverridePrimaryTargetEnabled", Direction.In),
				new ConnectionDesc("PrimaryTargetTagRestrictions", Direction.In),
				new ConnectionDesc("GlanceTargetsTagRestrictions", Direction.In)
			};
		}

		public ExtendedPA2BehaviorEntity(FrostySdk.Ebx.ExtendedPA2BehaviorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

