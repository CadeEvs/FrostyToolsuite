using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec3HubEntityData))]
	public class Vec3HubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec3HubEntityData>
	{
		public new FrostySdk.Ebx.Vec3HubEntityData Data => data as FrostySdk.Ebx.Vec3HubEntityData;
		public override string DisplayName => "Vec3Hub";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				for (int i = 0; i < Data.InputCount; i++)
                {
					outProperties.Add(new ConnectionDesc() { Name = $"In{i}", Direction = Direction.In });
                }
				outProperties.Add(new ConnectionDesc("Out", Direction.Out));
				return outProperties;
            }
		}

		public Vec3HubEntity(FrostySdk.Ebx.Vec3HubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

