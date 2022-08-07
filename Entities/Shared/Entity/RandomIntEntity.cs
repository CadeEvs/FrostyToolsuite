using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomIntEntityData))]
	public class RandomIntEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomIntEntityData>
	{
		public new FrostySdk.Ebx.RandomIntEntityData Data => data as FrostySdk.Ebx.RandomIntEntityData;
		public override string DisplayName => "RandomInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Out", Direction.Out)
			};
		}

        public RandomIntEntity(FrostySdk.Ebx.RandomIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

