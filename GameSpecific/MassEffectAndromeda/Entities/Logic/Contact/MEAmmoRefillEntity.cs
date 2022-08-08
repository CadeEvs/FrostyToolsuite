using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEAmmoRefillEntityData))]
	public class MEAmmoRefillEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEAmmoRefillEntityData>
	{
		public new FrostySdk.Ebx.MEAmmoRefillEntityData Data => data as FrostySdk.Ebx.MEAmmoRefillEntityData;
		public override string DisplayName => "MEAmmoRefill";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Entity", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Execute", Direction.In)
			};
		}

		public MEAmmoRefillEntity(FrostySdk.Ebx.MEAmmoRefillEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

