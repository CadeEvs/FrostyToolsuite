using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InterfacingTerminalData))]
	public class InterfacingTerminal : GameComponentEntity, IEntityData<FrostySdk.Ebx.InterfacingTerminalData>
	{
		public new FrostySdk.Ebx.InterfacingTerminalData Data => data as FrostySdk.Ebx.InterfacingTerminalData;

		public InterfacingTerminal(FrostySdk.Ebx.InterfacingTerminalData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

