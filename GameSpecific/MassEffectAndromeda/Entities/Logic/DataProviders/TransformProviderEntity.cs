using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformProviderEntityData))]
	public class TransformProviderEntity : ProviderEntity, IEntityData<FrostySdk.Ebx.TransformProviderEntityData>
	{
		public new FrostySdk.Ebx.TransformProviderEntityData Data => data as FrostySdk.Ebx.TransformProviderEntityData;
		public override string DisplayName => "TransformProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.Out)
			};
		}

        public TransformProviderEntity(FrostySdk.Ebx.TransformProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

