using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWGetTransformFromEventData))]
	public class BWGetTransformFromEvent : BWGetTransformBase, IEntityData<FrostySdk.Ebx.BWGetTransformFromEventData>
	{
		public new FrostySdk.Ebx.BWGetTransformFromEventData Data => data as FrostySdk.Ebx.BWGetTransformFromEventData;
		public override string DisplayName => "BWGetTransformFromEvent";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("WorldTransform", Direction.Out)
			};
		}

		public BWGetTransformFromEvent(FrostySdk.Ebx.BWGetTransformFromEventData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

