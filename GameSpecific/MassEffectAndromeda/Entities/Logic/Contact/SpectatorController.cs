using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpectatorControllerData))]
	public class SpectatorController : TrackableController, IEntityData<FrostySdk.Ebx.SpectatorControllerData>
	{
		public new FrostySdk.Ebx.SpectatorControllerData Data => data as FrostySdk.Ebx.SpectatorControllerData;
		public override string DisplayName => "SpectatorController";

		public SpectatorController(FrostySdk.Ebx.SpectatorControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

