using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetActiveVisibilityGroupsEntityData))]
	public class SetActiveVisibilityGroupsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetActiveVisibilityGroupsEntityData>
	{
		public new FrostySdk.Ebx.SetActiveVisibilityGroupsEntityData Data => data as FrostySdk.Ebx.SetActiveVisibilityGroupsEntityData;
		public override string DisplayName => "SetActiveVisibilityGroups";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Set", Direction.In)
			};
		}

		public SetActiveVisibilityGroupsEntity(FrostySdk.Ebx.SetActiveVisibilityGroupsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

