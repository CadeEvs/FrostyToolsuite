using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AntiCheatTrackerData))]
	public class AntiCheatTracker : LogicEntity, IEntityData<FrostySdk.Ebx.AntiCheatTrackerData>
	{
		public new FrostySdk.Ebx.AntiCheatTrackerData Data => data as FrostySdk.Ebx.AntiCheatTrackerData;
		public override string DisplayName => "AntiCheatTracker";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Character", Direction.In)
			};
		}

		public AntiCheatTracker(FrostySdk.Ebx.AntiCheatTrackerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

