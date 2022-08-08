using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoolProviderEntityData))]
	public class BoolProviderEntity : ProviderEntity, IEntityData<FrostySdk.Ebx.BoolProviderEntityData>
	{
		public new FrostySdk.Ebx.BoolProviderEntityData Data => data as FrostySdk.Ebx.BoolProviderEntityData;
		public override string DisplayName => "BoolProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("UpdateValue", Direction.In),
				new ConnectionDesc("OnTrue", Direction.Out),
				new ConnectionDesc("OnFalse", Direction.Out)
			};
        }
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.Out)
			};
		}

		public BoolProviderEntity(FrostySdk.Ebx.BoolProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

