using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSCapturePointEntityData))]
	public class WSCapturePointEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSCapturePointEntityData>
	{
		public new FrostySdk.Ebx.WSCapturePointEntityData Data => data as FrostySdk.Ebx.WSCapturePointEntityData;
		public override string DisplayName => "WSCapturePoint";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSCapturePointEntity(FrostySdk.Ebx.WSCapturePointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

