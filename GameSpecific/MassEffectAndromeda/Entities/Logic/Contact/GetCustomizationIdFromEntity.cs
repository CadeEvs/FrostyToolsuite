using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetCustomizationIdFromEntityData))]
	public class GetCustomizationIdFromEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetCustomizationIdFromEntityData>
	{
		public new FrostySdk.Ebx.GetCustomizationIdFromEntityData Data => data as FrostySdk.Ebx.GetCustomizationIdFromEntityData;
		public override string DisplayName => "GetCustomizationIdFrom";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Soldier", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Set", Direction.In),
				new ConnectionDesc("OnSet", Direction.Out)
			};
		}

		public GetCustomizationIdFromEntity(FrostySdk.Ebx.GetCustomizationIdFromEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

