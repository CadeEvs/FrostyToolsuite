using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AbsEntityData))]
	public class AbsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AbsEntityData>
	{
		public new FrostySdk.Ebx.AbsEntityData Data => data as FrostySdk.Ebx.AbsEntityData;
		public override string DisplayName => "Abs";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AbsEntity(FrostySdk.Ebx.AbsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

