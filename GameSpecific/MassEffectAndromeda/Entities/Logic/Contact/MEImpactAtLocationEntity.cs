using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEImpactAtLocationEntityData))]
	public class MEImpactAtLocationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEImpactAtLocationEntityData>
	{
		public new FrostySdk.Ebx.MEImpactAtLocationEntityData Data => data as FrostySdk.Ebx.MEImpactAtLocationEntityData;
		public override string DisplayName => "MEImpactAtLocation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ImpactGiver", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("ImpactTransform", Direction.In)
			};
		}

        public MEImpactAtLocationEntity(FrostySdk.Ebx.MEImpactAtLocationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

