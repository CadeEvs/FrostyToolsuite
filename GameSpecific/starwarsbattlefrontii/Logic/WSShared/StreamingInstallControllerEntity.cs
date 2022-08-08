using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StreamingInstallControllerEntityData))]
	public class StreamingInstallControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StreamingInstallControllerEntityData>
	{
		public new FrostySdk.Ebx.StreamingInstallControllerEntityData Data => data as FrostySdk.Ebx.StreamingInstallControllerEntityData;
		public override string DisplayName => "StreamingInstallController";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StreamingInstallControllerEntity(FrostySdk.Ebx.StreamingInstallControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

