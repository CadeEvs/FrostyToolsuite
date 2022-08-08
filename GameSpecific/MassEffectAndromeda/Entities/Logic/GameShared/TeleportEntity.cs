using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TeleportEntityData))]
	public class TeleportEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TeleportEntityData>
	{
		public new FrostySdk.Ebx.TeleportEntityData Data => data as FrostySdk.Ebx.TeleportEntityData;
		public override string DisplayName => "Teleport";
        public override IEnumerable<ConnectionDesc> Links
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("NewPosition", Direction.In)
            };
        }
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Teleport", Direction.In)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("NewPositionTransform", Direction.In)
            };
        }

        public TeleportEntity(FrostySdk.Ebx.TeleportEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

