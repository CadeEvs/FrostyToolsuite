using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalToWorldTransformEntityData))]
	public class LocalToWorldTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalToWorldTransformEntityData>
	{
		public new FrostySdk.Ebx.LocalToWorldTransformEntityData Data => data as FrostySdk.Ebx.LocalToWorldTransformEntityData;
		public override string DisplayName => "LocalToWorldTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("WorldTransform", Direction.Out)
			};
		}

		public LocalToWorldTransformEntity(FrostySdk.Ebx.LocalToWorldTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

