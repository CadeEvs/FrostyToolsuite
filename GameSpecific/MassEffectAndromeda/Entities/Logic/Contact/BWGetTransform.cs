using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWGetTransformData))]
	public class BWGetTransform : BWGetTransformBase, IEntityData<FrostySdk.Ebx.BWGetTransformData>
	{
		public new FrostySdk.Ebx.BWGetTransformData Data => data as FrostySdk.Ebx.BWGetTransformData;
		public override string DisplayName => "BWGetTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("SpatialEntity", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("GetTransform", Direction.In),
				new ConnectionDesc("OnDone", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("WorldTransform", Direction.Out)
			};
		}

		public BWGetTransform(FrostySdk.Ebx.BWGetTransformData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

