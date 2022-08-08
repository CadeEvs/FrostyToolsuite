using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BitwiseAndEntityData))]
	public class BitwiseAndEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BitwiseAndEntityData>
	{
		public new FrostySdk.Ebx.BitwiseAndEntityData Data => data as FrostySdk.Ebx.BitwiseAndEntityData;
		public override string DisplayName => "BitwiseAnd";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BitwiseAndEntity(FrostySdk.Ebx.BitwiseAndEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

