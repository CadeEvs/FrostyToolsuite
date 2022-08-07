using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectBoolEntityData))]
	public class SelectBoolEntity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectBoolEntityData>
	{
		public new FrostySdk.Ebx.SelectBoolEntityData Data => data as FrostySdk.Ebx.SelectBoolEntityData;
		public override string DisplayName => "SelectBool";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				outProperties.AddRange(base.Properties);
				outProperties.Add(new ConnectionDesc("Out", Direction.Out));
				return outProperties;
			}
		}

		public SelectBoolEntity(FrostySdk.Ebx.SelectBoolEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

