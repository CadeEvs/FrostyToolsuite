using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntegerProviderEntityData))]
	public class IntegerProviderEntity : ProviderEntity, IEntityData<FrostySdk.Ebx.IntegerProviderEntityData>
	{
		public new FrostySdk.Ebx.IntegerProviderEntityData Data => data as FrostySdk.Ebx.IntegerProviderEntityData;
		public override string DisplayName => "IntegerProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.Out)
			};
		}

        public IntegerProviderEntity(FrostySdk.Ebx.IntegerProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

