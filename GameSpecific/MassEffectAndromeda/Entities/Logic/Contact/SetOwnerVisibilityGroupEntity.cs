using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetOwnerVisibilityGroupEntityData))]
	public class SetOwnerVisibilityGroupEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetOwnerVisibilityGroupEntityData>
	{
		public new FrostySdk.Ebx.SetOwnerVisibilityGroupEntityData Data => data as FrostySdk.Ebx.SetOwnerVisibilityGroupEntityData;
		public override string DisplayName => "SetOwnerVisibilityGroup";
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Set", Direction.In)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("DisableOnGameView", Direction.In),
                new ConnectionDesc("VisibilityGroup", Direction.In)
            };
        }

        public SetOwnerVisibilityGroupEntity(FrostySdk.Ebx.SetOwnerVisibilityGroupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

