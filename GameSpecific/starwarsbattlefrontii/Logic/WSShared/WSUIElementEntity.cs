using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUIElementEntityData))]
	public class WSUIElementEntity : UIElementEntity, IEntityData<FrostySdk.Ebx.WSUIElementEntityData>
	{
		public new FrostySdk.Ebx.WSUIElementEntityData Data => data as FrostySdk.Ebx.WSUIElementEntityData;
		public override string DisplayName => "WSUIElement";

		public WSUIElementEntity(FrostySdk.Ebx.WSUIElementEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

