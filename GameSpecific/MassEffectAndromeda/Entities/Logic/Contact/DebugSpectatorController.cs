using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugSpectatorControllerData))]
	public class DebugSpectatorController : SpectatorController, IEntityData<FrostySdk.Ebx.DebugSpectatorControllerData>
	{
		public new FrostySdk.Ebx.DebugSpectatorControllerData Data => data as FrostySdk.Ebx.DebugSpectatorControllerData;
		public override string DisplayName => "DebugSpectatorController";

		public DebugSpectatorController(FrostySdk.Ebx.DebugSpectatorControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

