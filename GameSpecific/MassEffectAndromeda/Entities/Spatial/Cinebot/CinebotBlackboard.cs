using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotBlackboardData))]
	public class CinebotBlackboard : GameComponentEntity, IEntityData<FrostySdk.Ebx.CinebotBlackboardData>
	{
		public new FrostySdk.Ebx.CinebotBlackboardData Data => data as FrostySdk.Ebx.CinebotBlackboardData;

		public CinebotBlackboard(FrostySdk.Ebx.CinebotBlackboardData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

