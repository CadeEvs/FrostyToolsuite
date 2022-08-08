using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartyMemberAreaTriggerEntityData))]
	public class PartyMemberAreaTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.PartyMemberAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.PartyMemberAreaTriggerEntityData Data => data as FrostySdk.Ebx.PartyMemberAreaTriggerEntityData;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.AddRange(new[]
				{
					new ConnectionDesc("Radius", Direction.In),
					new ConnectionDesc("RadiusTransform", Direction.In)
				});
				return outProperties;
			}
		}

		public PartyMemberAreaTriggerEntity(FrostySdk.Ebx.PartyMemberAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

