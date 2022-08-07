using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomDelayEntityData))]
	public class RandomDelayEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomDelayEntityData>
	{
		public new FrostySdk.Ebx.RandomDelayEntityData Data => data as FrostySdk.Ebx.RandomDelayEntityData;
		public override string DisplayName => "RandomDelay";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
		}

		public RandomDelayEntity(FrostySdk.Ebx.RandomDelayEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

