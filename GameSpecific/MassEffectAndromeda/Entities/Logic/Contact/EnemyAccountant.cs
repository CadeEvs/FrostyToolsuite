using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnemyAccountantData))]
	public class EnemyAccountant : LogicEntity, IEntityData<FrostySdk.Ebx.EnemyAccountantData>
	{
		public new FrostySdk.Ebx.EnemyAccountantData Data => data as FrostySdk.Ebx.EnemyAccountantData;
		public override string DisplayName => "EnemyAccountant";

		public EnemyAccountant(FrostySdk.Ebx.EnemyAccountantData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

