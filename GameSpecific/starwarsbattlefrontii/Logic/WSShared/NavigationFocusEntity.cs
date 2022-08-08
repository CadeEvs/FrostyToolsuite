using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NavigationFocusEntityData))]
	public class NavigationFocusEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NavigationFocusEntityData>
	{
		public new FrostySdk.Ebx.NavigationFocusEntityData Data => data as FrostySdk.Ebx.NavigationFocusEntityData;
		public override string DisplayName => "NavigationFocus";

		public NavigationFocusEntity(FrostySdk.Ebx.NavigationFocusEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

