using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalStateEntityData))]
	public class ConditionalStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConditionalStateEntityData>
	{
		protected readonly int Property_Condition = Frosty.Hash.Fnv1.HashString("Condition");

		public new FrostySdk.Ebx.ConditionalStateEntityData Data => data as FrostySdk.Ebx.ConditionalStateEntityData;
		public override string DisplayName => "ConditionalState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Condition", Direction.In)
			};
		}

		protected Property<bool> conditionProperty;

		public ConditionalStateEntity(FrostySdk.Ebx.ConditionalStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			conditionProperty = new Property<bool>(this, Property_Condition, Data.Condition);
		}
	}
}

