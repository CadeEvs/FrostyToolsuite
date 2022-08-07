using System;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RandomFloatEntityData))]
	public class RandomFloatEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RandomFloatEntityData>
	{
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.RandomFloatEntityData Data => data as FrostySdk.Ebx.RandomFloatEntityData;
		public override string DisplayName => "RandomFloat";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Out", Direction.Out)
			};
		}

		protected Property<float> outProperty;
		protected static Random rand;

		public RandomFloatEntity(FrostySdk.Ebx.RandomFloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			outProperty = new Property<float>(this, Property_Out);
			if (rand == null)
			{
				rand = new Random();
			}
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();
			outProperty.Value = (float)rand.NextDouble();
        }
    }
}

