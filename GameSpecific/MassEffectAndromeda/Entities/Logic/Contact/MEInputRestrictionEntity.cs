using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEInputRestrictionEntityData))]
	public class MEInputRestrictionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEInputRestrictionEntityData>
	{
		public new FrostySdk.Ebx.MEInputRestrictionEntityData Data => data as FrostySdk.Ebx.MEInputRestrictionEntityData;
		public override string DisplayName => "MEInputRestriction";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Activate", Direction.In),
				new ConnectionDesc("Deactivate", Direction.In)
			};
		}

		public MEInputRestrictionEntity(FrostySdk.Ebx.MEInputRestrictionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

