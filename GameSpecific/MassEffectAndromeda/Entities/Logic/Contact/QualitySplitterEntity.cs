using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.QualitySplitterEntityData))]
	public class QualitySplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.QualitySplitterEntityData>
	{
		public new FrostySdk.Ebx.QualitySplitterEntityData Data => data as FrostySdk.Ebx.QualitySplitterEntityData;
		public override string DisplayName => "QualitySplitter";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Impulse", Direction.In)
			};
		}

		public QualitySplitterEntity(FrostySdk.Ebx.QualitySplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

