using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vector4EntityData))]
	public class Vector4Entity : LogicEntity, IEntityData<FrostySdk.Ebx.Vector4EntityData>
	{
		public new FrostySdk.Ebx.Vector4EntityData Data => data as FrostySdk.Ebx.Vector4EntityData;
		public override string DisplayName => "Vector4";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public Vector4Entity(FrostySdk.Ebx.Vector4EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

