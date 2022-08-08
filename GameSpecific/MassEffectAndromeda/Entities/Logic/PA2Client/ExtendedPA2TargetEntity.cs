using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExtendedPA2TargetEntityData))]
	public class ExtendedPA2TargetEntity : PA2TargetEntity, IEntityData<FrostySdk.Ebx.ExtendedPA2TargetEntityData>
	{
		public new FrostySdk.Ebx.ExtendedPA2TargetEntityData Data => data as FrostySdk.Ebx.ExtendedPA2TargetEntityData;
		public override string DisplayName => "ExtendedPA2Target";
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enable", Direction.In),
				new ConnectionDesc("Disable", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Transform", Direction.In),
				new ConnectionDesc("Radius", Direction.In),
				new ConnectionDesc("Enabled", Direction.In),
				new ConnectionDesc("Priority", Direction.In),
				new ConnectionDesc("Tags", Direction.In)
			};
		}

        public ExtendedPA2TargetEntity(FrostySdk.Ebx.ExtendedPA2TargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

