using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CompareIntRangeEntityData))]
	public class CompareIntRangeEntity : CompareEntityBase, IEntityData<FrostySdk.Ebx.CompareIntRangeEntityData>
	{
		public new FrostySdk.Ebx.CompareIntRangeEntityData Data => data as FrostySdk.Ebx.CompareIntRangeEntityData;
		public override string DisplayName => "CompareIntRange";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In),
				new ConnectionDesc("InRange", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.In)
			};
		}

		public CompareIntRangeEntity(FrostySdk.Ebx.CompareIntRangeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

