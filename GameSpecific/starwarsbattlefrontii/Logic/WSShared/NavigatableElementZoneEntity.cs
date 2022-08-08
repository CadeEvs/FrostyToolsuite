using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NavigatableElementZoneEntityData))]
	public class NavigatableElementZoneEntity : NavigatableElementEntity, IEntityData<FrostySdk.Ebx.NavigatableElementZoneEntityData>
	{
		public new FrostySdk.Ebx.NavigatableElementZoneEntityData Data => data as FrostySdk.Ebx.NavigatableElementZoneEntityData;
		public override string DisplayName => "NavigatableElementZone";

		public NavigatableElementZoneEntity(FrostySdk.Ebx.NavigatableElementZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

