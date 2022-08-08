using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProviderEntityData))]
	public class ProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ProviderEntityData>
	{
		public new FrostySdk.Ebx.ProviderEntityData Data => data as FrostySdk.Ebx.ProviderEntityData;
		public override string DisplayName => "Provider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Context", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("UpdateValue", Direction.In)
			};
		}

        public ProviderEntity(FrostySdk.Ebx.ProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

