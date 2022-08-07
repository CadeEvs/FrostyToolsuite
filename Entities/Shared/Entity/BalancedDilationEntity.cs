using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BalancedDilationEntityData))]
	public class BalancedDilationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BalancedDilationEntityData>
	{
		public new FrostySdk.Ebx.BalancedDilationEntityData Data => data as FrostySdk.Ebx.BalancedDilationEntityData;
		public override string DisplayName => "BalancedDilation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BalancedDilationEntity(FrostySdk.Ebx.BalancedDilationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

