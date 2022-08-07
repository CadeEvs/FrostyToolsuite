using System.Collections.Generic;
using Vec3 = FrostySdk.Ebx.Vec3;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vector3EntityData))]
	public class Vector3Entity : LogicEntity, IEntityData<FrostySdk.Ebx.Vector3EntityData>
	{
		protected readonly int Property_Vector = Frosty.Hash.Fnv1.HashString("Vector");
		public new FrostySdk.Ebx.Vector3EntityData Data => data as FrostySdk.Ebx.Vector3EntityData;
		public override string DisplayName => "Vector3";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Vector", Direction.Out)
			};
		}
		public override IEnumerable<string> HeaderRows
        {
			get => new List<string>()
			{
				$"DefaultVec3: ({Data.DefaultVec3.x}/{Data.DefaultVec3.y}/{Data.DefaultVec3.z})"
			};
        }

		protected Property<Vec3> vectorProperty;

        public Vector3Entity(FrostySdk.Ebx.Vector3EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			vectorProperty = new Property<Vec3>(this, Property_Vector);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			if (vectorProperty.IsUnset)
			{
				vectorProperty.Value = Data.DefaultVec3;
			}
        }
    }
}

