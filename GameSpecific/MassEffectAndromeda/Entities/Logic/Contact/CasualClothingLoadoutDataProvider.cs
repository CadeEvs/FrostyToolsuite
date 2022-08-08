using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CasualClothingLoadoutDataProviderData))]
	public class CasualClothingLoadoutDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.CasualClothingLoadoutDataProviderData>
	{
		public new FrostySdk.Ebx.CasualClothingLoadoutDataProviderData Data => data as FrostySdk.Ebx.CasualClothingLoadoutDataProviderData;
		public override string DisplayName => "CasualClothingLoadoutDataProvider";

		public CasualClothingLoadoutDataProvider(FrostySdk.Ebx.CasualClothingLoadoutDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

