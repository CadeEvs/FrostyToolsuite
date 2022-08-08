using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ArcBallEntityData))]
	public class ArcBallEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ArcBallEntityData>
	{
		public new FrostySdk.Ebx.ArcBallEntityData Data => data as FrostySdk.Ebx.ArcBallEntityData;
		public override string DisplayName => "ArcBall";

		public ArcBallEntity(FrostySdk.Ebx.ArcBallEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

