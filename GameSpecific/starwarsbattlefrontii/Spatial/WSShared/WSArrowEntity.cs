using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSArrowEntityData))]
	public class WSArrowEntity : WSBulletEntity, IEntityData<FrostySdk.Ebx.WSArrowEntityData>
	{
		public new FrostySdk.Ebx.WSArrowEntityData Data => data as FrostySdk.Ebx.WSArrowEntityData;

		public WSArrowEntity(FrostySdk.Ebx.WSArrowEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

