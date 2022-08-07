using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec2BuilderEntityData))]
	public class Vec2BuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec2BuilderEntityData>
	{
		public new FrostySdk.Ebx.Vec2BuilderEntityData Data => data as FrostySdk.Ebx.Vec2BuilderEntityData;
		public override string DisplayName => "Vec2Builder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("X", Direction.In),
				new ConnectionDesc("Y", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
		}

		public Vec2BuilderEntity(FrostySdk.Ebx.Vec2BuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

