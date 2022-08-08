using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetObjectTransformEntityData))]
	public class GetObjectTransformEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetObjectTransformEntityData>
	{
		public new FrostySdk.Ebx.GetObjectTransformEntityData Data => data as FrostySdk.Ebx.GetObjectTransformEntityData;
		public override string DisplayName => "GetObjectTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Entity", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("GetTransform", Direction.In),
				new ConnectionDesc("OnGetTransform", Direction.Out)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Transform", Direction.Out)
			};
		}

        public GetObjectTransformEntity(FrostySdk.Ebx.GetObjectTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

