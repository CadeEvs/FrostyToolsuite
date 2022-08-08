using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECSoldierVisibilityCheckerData))]
	public class MECSoldierVisibilityChecker : LogicEntity, IEntityData<FrostySdk.Ebx.MECSoldierVisibilityCheckerData>
	{
		public new FrostySdk.Ebx.MECSoldierVisibilityCheckerData Data => data as FrostySdk.Ebx.MECSoldierVisibilityCheckerData;
		public override string DisplayName => "MECSoldierVisibilityChecker";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Visible", Direction.Out),
				new ConnectionDesc("NotVisible", Direction.Out)
			};
		}

		public MECSoldierVisibilityChecker(FrostySdk.Ebx.MECSoldierVisibilityCheckerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

