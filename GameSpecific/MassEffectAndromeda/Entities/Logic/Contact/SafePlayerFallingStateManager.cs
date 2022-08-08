using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SafePlayerFallingStateManagerData))]
	public class SafePlayerFallingStateManager : LogicEntity, IEntityData<FrostySdk.Ebx.SafePlayerFallingStateManagerData>
	{
		public new FrostySdk.Ebx.SafePlayerFallingStateManagerData Data => data as FrostySdk.Ebx.SafePlayerFallingStateManagerData;
		public override string DisplayName => "SafePlayerFallingStateManager";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enable", Direction.In)
			};
		}

		public SafePlayerFallingStateManager(FrostySdk.Ebx.SafePlayerFallingStateManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

