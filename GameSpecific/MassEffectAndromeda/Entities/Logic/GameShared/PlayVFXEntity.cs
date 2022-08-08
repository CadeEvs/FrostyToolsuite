using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayVFXEntityData))]
	public class PlayVFXEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayVFXEntityData>
	{
		public new FrostySdk.Ebx.PlayVFXEntityData Data => data as FrostySdk.Ebx.PlayVFXEntityData;
		public override string DisplayName => "PlayVFX";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Start", Direction.In),
				new ConnectionDesc("Stop", Direction.In)
			};
		}

		public PlayVFXEntity(FrostySdk.Ebx.PlayVFXEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

