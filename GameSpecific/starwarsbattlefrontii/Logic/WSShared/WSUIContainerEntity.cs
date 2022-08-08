using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUIContainerEntityData))]
	public class WSUIContainerEntity : UIContainerEntity, IEntityData<FrostySdk.Ebx.WSUIContainerEntityData>
	{
		public new FrostySdk.Ebx.WSUIContainerEntityData Data => data as FrostySdk.Ebx.WSUIContainerEntityData;
		public override string DisplayName => "WSUIContainer";

		public WSUIContainerEntity(FrostySdk.Ebx.WSUIContainerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

