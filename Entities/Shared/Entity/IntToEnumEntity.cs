using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntToEnumEntityData))]
	public class IntToEnumEntity : ExplicitEnumTypeLogicEntity, IEntityData<FrostySdk.Ebx.IntToEnumEntityData>
	{
		public new FrostySdk.Ebx.IntToEnumEntityData Data => data as FrostySdk.Ebx.IntToEnumEntityData;
		public override string DisplayName => $"IntToEnum";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
		}
		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				if (enumType != null)
				{
					outHeaderRows.Add(enumType.Name);
				}
				return outHeaderRows;
			}
		}

		public IntToEnumEntity(FrostySdk.Ebx.IntToEnumEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

