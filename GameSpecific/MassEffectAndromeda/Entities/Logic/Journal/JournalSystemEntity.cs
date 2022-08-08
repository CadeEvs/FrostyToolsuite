using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JournalSystemEntityData))]
	public class JournalSystemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.JournalSystemEntityData>
	{
		public new FrostySdk.Ebx.JournalSystemEntityData Data => data as FrostySdk.Ebx.JournalSystemEntityData;
		public override string DisplayName => "JournalSystem";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Initialize", Direction.In)
			};
		}

		public JournalSystemEntity(FrostySdk.Ebx.JournalSystemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

