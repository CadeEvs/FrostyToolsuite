using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformSelectorEntityData))]
	public class TransformSelectorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformSelectorEntityData>
	{
		public new FrostySdk.Ebx.TransformSelectorEntityData Data => data as FrostySdk.Ebx.TransformSelectorEntityData;
		public override string DisplayName => "TransformSelector";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In1", Direction.In),
				new ConnectionDesc("In2", Direction.In),
				new ConnectionDesc("Selection", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
		}

		public TransformSelectorEntity(FrostySdk.Ebx.TransformSelectorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

