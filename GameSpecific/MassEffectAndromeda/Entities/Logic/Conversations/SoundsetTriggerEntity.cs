using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundsetTriggerEntityData))]
	public class SoundsetTriggerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundsetTriggerEntityData>
	{
		public new FrostySdk.Ebx.SoundsetTriggerEntityData Data => data as FrostySdk.Ebx.SoundsetTriggerEntityData;
		public override string DisplayName => "SoundsetTrigger";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Character", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Start", Direction.In)
			};
		}

		public SoundsetTriggerEntity(FrostySdk.Ebx.SoundsetTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

