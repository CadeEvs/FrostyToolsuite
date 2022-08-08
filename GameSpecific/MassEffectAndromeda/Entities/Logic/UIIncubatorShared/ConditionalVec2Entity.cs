using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConditionalVec2EntityData))]
	public class ConditionalVec2Entity : ConditionalStateEntity, IEntityData<FrostySdk.Ebx.ConditionalVec2EntityData>
	{
		public new FrostySdk.Ebx.ConditionalVec2EntityData Data => data as FrostySdk.Ebx.ConditionalVec2EntityData;
		public override string DisplayName => "ConditionalVec2";
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

		public ConditionalVec2Entity(FrostySdk.Ebx.ConditionalVec2EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

