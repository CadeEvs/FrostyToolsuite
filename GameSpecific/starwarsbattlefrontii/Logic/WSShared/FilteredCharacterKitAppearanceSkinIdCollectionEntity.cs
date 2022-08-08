using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FilteredCharacterKitAppearanceSkinIdCollectionEntityData))]
	public class FilteredCharacterKitAppearanceSkinIdCollectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FilteredCharacterKitAppearanceSkinIdCollectionEntityData>
	{
		public new FrostySdk.Ebx.FilteredCharacterKitAppearanceSkinIdCollectionEntityData Data => data as FrostySdk.Ebx.FilteredCharacterKitAppearanceSkinIdCollectionEntityData;
		public override string DisplayName => "FilteredCharacterKitAppearanceSkinIdCollection";

		public FilteredCharacterKitAppearanceSkinIdCollectionEntity(FrostySdk.Ebx.FilteredCharacterKitAppearanceSkinIdCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

