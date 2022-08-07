using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformBuilderEntityData))]
	public class TransformBuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformBuilderEntityData>
	{
		public new FrostySdk.Ebx.TransformBuilderEntityData Data => data as FrostySdk.Ebx.TransformBuilderEntityData;
		public override string DisplayName => "TransformBuilder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Left", Direction.In),
				new ConnectionDesc("Up", Direction.In),
				new ConnectionDesc("Forward", Direction.In),
				new ConnectionDesc("Trans", Direction.In),
				new ConnectionDesc("Transform", Direction.Out)
			};
		}

		public TransformBuilderEntity(FrostySdk.Ebx.TransformBuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

