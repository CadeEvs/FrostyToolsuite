using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUIScreenRenderEntityData))]
	public class WSUIScreenRenderEntity : UIScreenRenderEntity, IEntityData<FrostySdk.Ebx.WSUIScreenRenderEntityData>
	{
		public new FrostySdk.Ebx.WSUIScreenRenderEntityData Data => data as FrostySdk.Ebx.WSUIScreenRenderEntityData;

		public WSUIScreenRenderEntity(FrostySdk.Ebx.WSUIScreenRenderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

