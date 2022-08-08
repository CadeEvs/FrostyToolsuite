using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEStreamingCacheEntityData))]
	public class MEStreamingCacheEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.MEStreamingCacheEntityData>
	{
		public new FrostySdk.Ebx.MEStreamingCacheEntityData Data => data as FrostySdk.Ebx.MEStreamingCacheEntityData;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Activate", Direction.In),
				new ConnectionDesc("Deactivate", Direction.In)
			};
		}

		public MEStreamingCacheEntity(FrostySdk.Ebx.MEStreamingCacheEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

