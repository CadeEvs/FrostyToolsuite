using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Or4EntityData))]
	public class Or4Entity : LogicEntity, IEntityData<FrostySdk.Ebx.Or4EntityData>
	{
		public new FrostySdk.Ebx.Or4EntityData Data => data as FrostySdk.Ebx.Or4EntityData;
		public override string DisplayName => "Or4";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Or4Entity(FrostySdk.Ebx.Or4EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

