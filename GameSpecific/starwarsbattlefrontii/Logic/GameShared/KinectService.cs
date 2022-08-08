using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KinectServiceData))]
	public class KinectService : LogicEntity, IEntityData<FrostySdk.Ebx.KinectServiceData>
	{
		public new FrostySdk.Ebx.KinectServiceData Data => data as FrostySdk.Ebx.KinectServiceData;
		public override string DisplayName => "KinectService";

		public KinectService(FrostySdk.Ebx.KinectServiceData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

