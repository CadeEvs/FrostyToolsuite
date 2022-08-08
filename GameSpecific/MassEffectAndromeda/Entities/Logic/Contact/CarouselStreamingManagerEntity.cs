using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CarouselStreamingManagerEntityData))]
	public class CarouselStreamingManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CarouselStreamingManagerEntityData>
	{
		public new FrostySdk.Ebx.CarouselStreamingManagerEntityData Data => data as FrostySdk.Ebx.CarouselStreamingManagerEntityData;
		public override string DisplayName => "CarouselStreamingManager";

		public CarouselStreamingManagerEntity(FrostySdk.Ebx.CarouselStreamingManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

