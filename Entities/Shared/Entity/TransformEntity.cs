using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformEntityData))]
	public class TransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformEntityData>
	{
		public new FrostySdk.Ebx.TransformEntityData Data => data as FrostySdk.Ebx.TransformEntityData;
		public override string DisplayName => "Transform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Transform", Direction.Out)
			};
		}

		public TransformEntity(FrostySdk.Ebx.TransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

