using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIScreenRenderTargetEntityData))]
	public class UIScreenRenderTargetEntity : UIScreenRenderEntity, IEntityData<FrostySdk.Ebx.UIScreenRenderTargetEntityData>
	{
		public new FrostySdk.Ebx.UIScreenRenderTargetEntityData Data => data as FrostySdk.Ebx.UIScreenRenderTargetEntityData;

		public UIScreenRenderTargetEntity(FrostySdk.Ebx.UIScreenRenderTargetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

