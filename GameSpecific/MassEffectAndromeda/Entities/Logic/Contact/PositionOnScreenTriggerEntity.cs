using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PositionOnScreenTriggerEntityData))]
	public class PositionOnScreenTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PositionOnScreenTriggerEntityData>
	{
		public new FrostySdk.Ebx.PositionOnScreenTriggerEntityData Data => data as FrostySdk.Ebx.PositionOnScreenTriggerEntityData;
		public override string DisplayName => "PositionOnScreenTrigger";
        public override IEnumerable<ConnectionDesc> Links
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("TargetEntity", Direction.In),
                new ConnectionDesc("ExcludedEntities", Direction.In)
            };
        }
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Enable", Direction.In),
                new ConnectionDesc("Disable", Direction.In),
                new ConnectionDesc("OnTrigger", Direction.Out)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("StartEnabled", Direction.In),
                new ConnectionDesc("DisableOnTrigger", Direction.In),
                new ConnectionDesc("OnScreenTreshold", Direction.In),
                new ConnectionDesc("DontCheckWater", Direction.In),
                new ConnectionDesc("DontCheckTerrain", Direction.In),
                new ConnectionDesc("DontCheckRagdoll", Direction.In),
                new ConnectionDesc("DontCheckCharacter", Direction.In),
                new ConnectionDesc("DontCheckGroup", Direction.In),
                new ConnectionDesc("DontCheckPhantoms", Direction.In)
            };
        }

        public PositionOnScreenTriggerEntity(FrostySdk.Ebx.PositionOnScreenTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

