using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SwitchPropertyStringEntityData))]
	public class SwitchPropertyStringEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SwitchPropertyStringEntityData>
	{
		public new FrostySdk.Ebx.SwitchPropertyStringEntityData Data => data as FrostySdk.Ebx.SwitchPropertyStringEntityData;
		public override string DisplayName => "SwitchPropertyString";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("StringOut", Direction.Out)
			};
		}

		public SwitchPropertyStringEntity(FrostySdk.Ebx.SwitchPropertyStringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

