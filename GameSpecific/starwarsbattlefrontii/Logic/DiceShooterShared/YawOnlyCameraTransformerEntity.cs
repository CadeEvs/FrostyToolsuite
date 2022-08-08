using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.YawOnlyCameraTransformerEntityData))]
	public class YawOnlyCameraTransformerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.YawOnlyCameraTransformerEntityData>
	{
		public new FrostySdk.Ebx.YawOnlyCameraTransformerEntityData Data => data as FrostySdk.Ebx.YawOnlyCameraTransformerEntityData;
		public override string DisplayName => "YawOnlyCameraTransformer";

		public YawOnlyCameraTransformerEntity(FrostySdk.Ebx.YawOnlyCameraTransformerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

