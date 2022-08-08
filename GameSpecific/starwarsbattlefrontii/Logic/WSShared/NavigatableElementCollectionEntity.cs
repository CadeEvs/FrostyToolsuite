using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NavigatableElementCollectionEntityData))]
	public class NavigatableElementCollectionEntity : NavigatableElementEntity, IEntityData<FrostySdk.Ebx.NavigatableElementCollectionEntityData>
	{
		public new FrostySdk.Ebx.NavigatableElementCollectionEntityData Data => data as FrostySdk.Ebx.NavigatableElementCollectionEntityData;
		public override string DisplayName => "NavigatableElementCollection";

		public NavigatableElementCollectionEntity(FrostySdk.Ebx.NavigatableElementCollectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

