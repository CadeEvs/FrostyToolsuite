using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrigonometryEntityData))]
	public class TrigonometryEntity : SimpleEntity, IEntityData<FrostySdk.Ebx.TrigonometryEntityData>
	{
		public new FrostySdk.Ebx.TrigonometryEntityData Data => data as FrostySdk.Ebx.TrigonometryEntityData;
		public override string DisplayName => "Trigonometry";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Angle", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
		}

		public TrigonometryEntity(FrostySdk.Ebx.TrigonometryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

