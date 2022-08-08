using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoReleaseUIInputEntityData))]
	public class AutoReleaseUIInputEntity : UIInputEntity, IEntityData<FrostySdk.Ebx.AutoReleaseUIInputEntityData>
	{
		public new FrostySdk.Ebx.AutoReleaseUIInputEntityData Data => data as FrostySdk.Ebx.AutoReleaseUIInputEntityData;
		public override string DisplayName => "AutoReleaseUIInput";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("TakeInput", Direction.In),
				new ConnectionDesc("ReleaseInput", Direction.In)
			};
		}

		public AutoReleaseUIInputEntity(FrostySdk.Ebx.AutoReleaseUIInputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

