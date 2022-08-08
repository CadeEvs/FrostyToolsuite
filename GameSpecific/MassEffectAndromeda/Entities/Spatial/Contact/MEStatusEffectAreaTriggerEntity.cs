using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEStatusEffectAreaTriggerEntityData))]
	public class MEStatusEffectAreaTriggerEntity : TriggerEntity, IEntityData<FrostySdk.Ebx.MEStatusEffectAreaTriggerEntityData>
	{
		public new FrostySdk.Ebx.MEStatusEffectAreaTriggerEntityData Data => data as FrostySdk.Ebx.MEStatusEffectAreaTriggerEntityData;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enable", Direction.In),
				new ConnectionDesc("Disable", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Radius", Direction.In));
				outProperties.Add(new ConnectionDesc("SourceItemHash", Direction.In));
				return outProperties;
            }
        }

        public MEStatusEffectAreaTriggerEntity(FrostySdk.Ebx.MEStatusEffectAreaTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

