using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NavigationInterfaceData))]
	public class NavigationInterface : LogicEntity, IEntityData<FrostySdk.Ebx.NavigationInterfaceData>
	{
		public new FrostySdk.Ebx.NavigationInterfaceData Data => data as FrostySdk.Ebx.NavigationInterfaceData;
		public override string DisplayName => "NavigationInterface";

		public NavigationInterface(FrostySdk.Ebx.NavigationInterfaceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

