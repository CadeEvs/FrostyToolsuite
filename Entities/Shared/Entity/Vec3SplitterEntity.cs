using System.Collections.Generic;
using Vec3 = FrostySdk.Ebx.Vec3;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec3SplitterEntityData))]
	public class Vec3SplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec3SplitterEntityData>
	{
		public new FrostySdk.Ebx.Vec3SplitterEntityData Data => data as FrostySdk.Ebx.Vec3SplitterEntityData;
		public override string DisplayName => "Vec3Splitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In, typeof(Vec3)),
				new ConnectionDesc("X", Direction.Out, typeof(float)),
				new ConnectionDesc("Y", Direction.Out, typeof(float)),
				new ConnectionDesc("Z", Direction.Out, typeof(float))
			};
		}

		public Vec3SplitterEntity(FrostySdk.Ebx.Vec3SplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

