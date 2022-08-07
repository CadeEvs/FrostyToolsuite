using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec4BuilderEntityData))]
	public class Vec4BuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec4BuilderEntityData>
	{
		public new FrostySdk.Ebx.Vec4BuilderEntityData Data => data as FrostySdk.Ebx.Vec4BuilderEntityData;
		public override string DisplayName => "Vec4Builder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("X", Direction.In),
				new ConnectionDesc("Y", Direction.In),
				new ConnectionDesc("Z", Direction.In),
				new ConnectionDesc("W", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
        }

        public Vec4BuilderEntity(FrostySdk.Ebx.Vec4BuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

