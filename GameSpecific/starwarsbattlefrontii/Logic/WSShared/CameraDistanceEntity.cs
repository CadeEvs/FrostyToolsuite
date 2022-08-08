using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CameraDistanceEntityData))]
	public class CameraDistanceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CameraDistanceEntityData>
	{
		public new FrostySdk.Ebx.CameraDistanceEntityData Data => data as FrostySdk.Ebx.CameraDistanceEntityData;
		public override string DisplayName => "CameraDistance";

		public CameraDistanceEntity(FrostySdk.Ebx.CameraDistanceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

