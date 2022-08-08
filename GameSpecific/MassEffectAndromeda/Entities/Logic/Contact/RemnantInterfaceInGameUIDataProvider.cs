using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemnantInterfaceInGameUIDataProviderData))]
	public class RemnantInterfaceInGameUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.RemnantInterfaceInGameUIDataProviderData>
	{
		public new FrostySdk.Ebx.RemnantInterfaceInGameUIDataProviderData Data => data as FrostySdk.Ebx.RemnantInterfaceInGameUIDataProviderData;
		public override string DisplayName => "RemnantInterfaceInGameUIDataProvider";

		public RemnantInterfaceInGameUIDataProvider(FrostySdk.Ebx.RemnantInterfaceInGameUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

