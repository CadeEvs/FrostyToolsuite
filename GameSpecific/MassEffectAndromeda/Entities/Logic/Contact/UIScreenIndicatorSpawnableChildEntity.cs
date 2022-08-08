using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIScreenIndicatorSpawnableChildEntityData))]
	public class UIScreenIndicatorSpawnableChildEntity : UIChildItemSpawnableChildEntity, IEntityData<FrostySdk.Ebx.UIScreenIndicatorSpawnableChildEntityData>
	{
		public new FrostySdk.Ebx.UIScreenIndicatorSpawnableChildEntityData Data => data as FrostySdk.Ebx.UIScreenIndicatorSpawnableChildEntityData;
		public override string DisplayName => "UIScreenIndicatorSpawnableChild";

		public UIScreenIndicatorSpawnableChildEntity(FrostySdk.Ebx.UIScreenIndicatorSpawnableChildEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

