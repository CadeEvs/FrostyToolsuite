using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnPowerTriggeredEntityData))]
	public class OnPowerTriggeredEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnPowerTriggeredEntityData>
	{
		public new FrostySdk.Ebx.OnPowerTriggeredEntityData Data => data as FrostySdk.Ebx.OnPowerTriggeredEntityData;
		public override string DisplayName => "OnPowerTriggered";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnPowerTriggered", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Power", Direction.Out)
			};
		}

        public OnPowerTriggeredEntity(FrostySdk.Ebx.OnPowerTriggeredEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

