using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIScreenRenderEntityData))]
	public class UIScreenRenderEntity : LogicReferenceObject, IEntityData<FrostySdk.Ebx.UIScreenRenderEntityData>
	{
		public new FrostySdk.Ebx.UIScreenRenderEntityData Data => data as FrostySdk.Ebx.UIScreenRenderEntityData;

		public UIScreenRenderEntity(FrostySdk.Ebx.UIScreenRenderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

