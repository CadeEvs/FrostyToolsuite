using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemnantInterfacePauseMenuUIDataProviderData))]
	public class RemnantInterfacePauseMenuUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.RemnantInterfacePauseMenuUIDataProviderData>
	{
		public new FrostySdk.Ebx.RemnantInterfacePauseMenuUIDataProviderData Data => data as FrostySdk.Ebx.RemnantInterfacePauseMenuUIDataProviderData;
		public override string DisplayName => "RemnantInterfacePauseMenuUIDataProvider";

		public RemnantInterfacePauseMenuUIDataProvider(FrostySdk.Ebx.RemnantInterfacePauseMenuUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

