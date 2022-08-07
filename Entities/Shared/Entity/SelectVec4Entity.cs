using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SelectVec4EntityData))]
	public class SelectVec4Entity : SelectPropertyEntity, IEntityData<FrostySdk.Ebx.SelectVec4EntityData>
	{
		public new FrostySdk.Ebx.SelectVec4EntityData Data => data as FrostySdk.Ebx.SelectVec4EntityData;
		public override string DisplayName => "SelectVec4";
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

		public SelectVec4Entity(FrostySdk.Ebx.SelectVec4EntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

