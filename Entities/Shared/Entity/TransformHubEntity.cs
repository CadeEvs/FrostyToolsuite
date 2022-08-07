using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformHubEntityData))]
	public class TransformHubEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformHubEntityData>
	{
		public new FrostySdk.Ebx.TransformHubEntityData Data => data as FrostySdk.Ebx.TransformHubEntityData;
		public override string DisplayName => "TransformHub";
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

		public TransformHubEntity(FrostySdk.Ebx.TransformHubEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

