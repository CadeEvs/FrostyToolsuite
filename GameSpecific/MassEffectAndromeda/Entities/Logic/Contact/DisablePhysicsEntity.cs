using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DisablePhysicsEntityData))]
	public class DisablePhysicsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DisablePhysicsEntityData>
	{
		public new FrostySdk.Ebx.DisablePhysicsEntityData Data => data as FrostySdk.Ebx.DisablePhysicsEntityData;
		public override string DisplayName => "DisablePhysics";
        public override IEnumerable<ConnectionDesc> Links
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Character", Direction.In)
            };
        }
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Stop", Direction.In)
            };
        }

        public DisablePhysicsEntity(FrostySdk.Ebx.DisablePhysicsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

