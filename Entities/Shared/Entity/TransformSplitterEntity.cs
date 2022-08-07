using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformSplitterEntityData))]
	public class TransformSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformSplitterEntityData>
	{
		public new FrostySdk.Ebx.TransformSplitterEntityData Data => data as FrostySdk.Ebx.TransformSplitterEntityData;
		public override string DisplayName => "TransformSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Transform", Direction.In),
				new ConnectionDesc("Right", Direction.Out),
				new ConnectionDesc("Up", Direction.Out),
				new ConnectionDesc("Forward", Direction.Out),
				new ConnectionDesc("Trans", Direction.Out)
			};
		}

		public TransformSplitterEntity(FrostySdk.Ebx.TransformSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

