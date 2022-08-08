using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneEntityData))]
	public class ZoneEntity : AreaTriggerBase, IEntityData<FrostySdk.Ebx.ZoneEntityData>
	{
		public new FrostySdk.Ebx.ZoneEntityData Data => data as FrostySdk.Ebx.ZoneEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Characters", Direction.In),
                new ConnectionDesc("Geometry", Direction.In)
            };
        }
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
                new ConnectionDesc("Enabled", Direction.In),
                new ConnectionDesc("GeometryTransform", Direction.In)
            };
        }

        public ZoneEntity(FrostySdk.Ebx.ZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

