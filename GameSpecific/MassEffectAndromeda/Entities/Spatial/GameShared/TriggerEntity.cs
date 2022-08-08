using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TriggerEntityData))]
	public class TriggerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.TriggerEntityData>
	{
		public new FrostySdk.Ebx.TriggerEntityData Data => data as FrostySdk.Ebx.TriggerEntityData;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.AddRange(new[]
				{
					new ConnectionDesc("Enabled", Direction.In)
				});
				return outProperties;
			}
		}

		public TriggerEntity(FrostySdk.Ebx.TriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

