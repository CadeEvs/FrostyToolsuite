using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NavigatableElementEntityData))]
	public class NavigatableElementEntity : WSUIElementEntity, IEntityData<FrostySdk.Ebx.NavigatableElementEntityData>
	{
		public new FrostySdk.Ebx.NavigatableElementEntityData Data => data as FrostySdk.Ebx.NavigatableElementEntityData;
		public override string DisplayName => "NavigatableElement";

		public NavigatableElementEntity(FrostySdk.Ebx.NavigatableElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

