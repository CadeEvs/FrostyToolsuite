using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalTransformEntityData))]
	public class ConditionalTransformEntity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalTransformEntityData>
	{
		public new FrostySdk.Ebx.ConditionalTransformEntityData Data => data as FrostySdk.Ebx.ConditionalTransformEntityData;
		public override string DisplayName => "ConditionalTransform";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("ValueIfTrue", Direction.In));
				outProperties.Add(new ConnectionDesc("ValueIfFalse", Direction.In));
				outProperties.Add(new ConnectionDesc("Output", Direction.Out));
				return outProperties;
			}
		}

		public ConditionalTransformEntity(FrostySdk.Ebx.ConditionalTransformEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

