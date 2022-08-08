using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectiveLocationMarkerComponentData))]
	public class ObjectiveLocationMarkerComponent : LocationMarkerComponent, IEntityData<FrostySdk.Ebx.ObjectiveLocationMarkerComponentData>
	{
		public new FrostySdk.Ebx.ObjectiveLocationMarkerComponentData Data => data as FrostySdk.Ebx.ObjectiveLocationMarkerComponentData;
		public override string DisplayName => "ObjectiveLocationMarkerComponent";

		public ObjectiveLocationMarkerComponent(FrostySdk.Ebx.ObjectiveLocationMarkerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

