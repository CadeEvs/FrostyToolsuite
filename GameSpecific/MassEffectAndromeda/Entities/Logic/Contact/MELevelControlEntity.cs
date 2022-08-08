using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MELevelControlEntityData))]
	public class MELevelControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MELevelControlEntityData>
	{
		public new FrostySdk.Ebx.MELevelControlEntityData Data => data as FrostySdk.Ebx.MELevelControlEntityData;
		public override string DisplayName => "MELevelControl";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("StreamIn", Direction.In),
				new ConnectionDesc("StreamOut", Direction.In)
			};
		}

		public MELevelControlEntity(FrostySdk.Ebx.MELevelControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

